using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum BotDifficulty { Easy, Medium, Hard }

public class BotAI : MonoBehaviour
{
    public BotDifficulty difficulty = BotDifficulty.Easy;

    public static bool IsBotTurn = false;

    private static readonly string[] Colors = { "Red", "Blue", "Yellow" };
    private const float ThinkDelay = 0.8f;

    // MCTS support
    private GameSimulator _sim;
    private BotAction?    _pendingMCTSAction; // set in HandleAbilityPhase, consumed in GenerateActions

    // Hard bot (PPO inference)
    private BotAgent _hardAgent;

    void Start()
    {
        if (difficulty == BotDifficulty.Medium || difficulty == BotDifficulty.Hard)
            _sim = GameSimulator.BuildFromScene();

        if (difficulty == BotDifficulty.Hard)
        {
            PlayerMover bot = GameManager.Instance?.players[1];
            if (bot != null)
            {
                // Components are disabled by default to avoid Sentis model compilation at scene load.
                // Enable them now that we know Hard difficulty is active.
                var bp = bot.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();
                if (bp != null) bp.enabled = true;

                _hardAgent = bot.GetComponent<BotAgent>();
                if (_hardAgent != null)
                {
                    _hardAgent.enabled = true;
                    _hardAgent.sim     = _sim;
                    _hardAgent.manager = null; // inference mode — no training manager
                }
            }
        }
    }

    // Called by ChoiceManager — runs coroutine on BotAI (always active) instead of ChoiceManager
    public void StartAbilityPhase(PlayerMover self, PlayerMover opponent, System.Action done)
        => StartCoroutine(HandleAbilityPhase(self, opponent, done));

    public void StartActionPhase()
        => StartCoroutine(SubmitBotActions());

    IEnumerator HandleAbilityPhase(PlayerMover self, PlayerMover opponent, System.Action done)
    {
        // For Medium: snapshot state and kick off MCTS on a background thread so it
        // runs concurrently with the ThinkDelay — total pause stays at ~ThinkDelay.
        BotAction? mctsResult = null;
        Thread mctsThread = null;

        if (difficulty == BotDifficulty.Medium && _sim != null)
        {
            GameStateSim snapshot = _sim.SnapshotCurrentState();
            mctsThread = new Thread(() =>
            {
                mctsResult = MCTSBot.GetBotAction(snapshot, _sim, budgetMs: 700);
            });
            mctsThread.IsBackground = true;
            mctsThread.Start();
        }

        yield return new WaitForSeconds(ThinkDelay);

        AbilityManager mgr = self.GetComponent<AbilityManager>();
        mgr?.OnTurnStart();

        if (difficulty == BotDifficulty.Medium && mctsThread != null)
        {
            mctsThread.Join(); // should be done by now (700ms budget vs 800ms delay)
            if (mctsResult.HasValue)
            {
                _pendingMCTSAction = mctsResult;
                if (mctsResult.Value.ability.HasValue && mgr != null)
                    mgr.UseAbility(mctsResult.Value.ability.Value, self, opponent);
            }
        }
        else if (difficulty == BotDifficulty.Hard && _hardAgent != null && _sim != null)
        {
            // PPO inference: snapshot → neural net → get action
            GameStateSim snapshot = _sim.SnapshotCurrentState();
            // Ensure state is from bot's (index 0) perspective
            if (snapshot.currentPlayerIndex != 0)
            {
                var tmp = snapshot.players[0];
                snapshot.players[0] = snapshot.players[1];
                snapshot.players[1] = tmp;
                snapshot.currentPlayerIndex = 0;
            }
            _hardAgent.obsState    = snapshot;
            _hardAgent.actionReady = false;
            _hardAgent.RequestDecision();

            // Wait up to 2 frames for inference (neural net is synchronous, should be 1 frame)
            yield return null;
            if (!_hardAgent.actionReady) yield return null;

            if (_hardAgent.actionReady)
            {
                _pendingMCTSAction = _hardAgent.lastAction;
                if (_hardAgent.lastAction.ability.HasValue && mgr != null)
                    mgr.UseAbility(_hardAgent.lastAction.ability.Value, self, opponent);
            }
        }

        done?.Invoke();
    }

    // Called internally — generates and submits actions
    IEnumerator SubmitBotActions()
    {
        yield return new WaitForSeconds(ThinkDelay);

        PlayerMover bot = GameManager.Instance.players[1];
        PlayerAction[] actions = GenerateActions(bot);
        IsBotTurn = true;
        // Run on BotAI (always active) — ChoiceManager may be inactive
        StartCoroutine(ChoiceManager.Instance.ExecutePlannedActions(actions));
    }

    // ── Action Generation ────────────────────────────────────────────────────

    PlayerAction[] GenerateActions(PlayerMover bot)
    {
        // Medium and Hard both cache their decision in _pendingMCTSAction
        if (_pendingMCTSAction.HasValue)
        {
            var pending = _pendingMCTSAction.Value;
            _pendingMCTSAction = null;

            bool hasAnyMove = pending.moves != null &&
                              System.Array.Exists(pending.moves, m => m != null);
            if (hasAnyMove)
                return ConvertBotAction(pending);

            // Model output no moves (ability-only action) — use heuristic for movement
            return GenerateHardActions(bot);
        }

        return difficulty switch
        {
            BotDifficulty.Medium => GenerateMediumActions(bot),
            BotDifficulty.Hard   => GenerateHardActions(bot),
            _                    => GenerateEasyActions()
        };
    }

    // Converts MCTS BotAction (string colors) to the PlayerAction[] the game expects.
    static PlayerAction[] ConvertBotAction(BotAction botAction)
    {
        var result = new PlayerAction[4];
        for (int i = 0; i < 4; i++)
        {
            PlayerAction.PathColor? color = null;
            string colorStr = botAction.moves != null && i < botAction.moves.Length ? botAction.moves[i] : null;
            if (colorStr != null && System.Enum.TryParse(colorStr, out PlayerAction.PathColor pc))
                color = pc;
            bool doAct = botAction.doActions != null && i < botAction.doActions.Length && botAction.doActions[i];
            result[i] = new PlayerAction(color, doAct);
        }
        return result;
    }

    PlayerAction[] GenerateEasyActions()
    {
        var actions = new PlayerAction[4];
        for (int i = 0; i < 4; i++)
        {
            string color = Colors[Random.Range(0, Colors.Length)];
            bool interact = Random.value > 0.5f;
            PlayerAction.PathColor? pc = System.Enum.TryParse(color, out PlayerAction.PathColor parsed) ? parsed : (PlayerAction.PathColor?)null;
            actions[i] = new PlayerAction(pc, interact);
        }
        return actions;
    }

    PlayerAction[] GenerateMediumActions(PlayerMover bot)
    {
        string target = FindNearestChest(bot.currentCityName);
        if (target == null) return GenerateEasyActions();

        return BuildActionsToward(bot, target);
    }

    PlayerAction[] GenerateHardActions(PlayerMover bot)
    {
        PlayerMover p1 = GameManager.Instance.players[0];
        string target = FindNearestChest(bot.currentCityName);

        // If we can defeat a tower, prioritize it
        Inventory inv = bot.GetComponent<Inventory>();
        if (inv != null)
        {
            int threshold = ClassData.TowerThreshold(bot.playerClass);
            if (inv.GetItemCount(Inventory.ItemType.Sword)  >= threshold &&
                inv.GetItemCount(Inventory.ItemType.Shield) >= threshold &&
                inv.GetItemCount(Inventory.ItemType.Bow)    >= threshold)
            {
                string towerCity = FindNearestTower(bot.currentCityName);
                if (towerCity != null) target = towerCity;
            }
        }

        // Race toward chests near player 1 if they're closer
        if (target == null) target = FindNearestChest(p1.currentCityName);
        if (target == null) return GenerateEasyActions();

        return BuildActionsToward(bot, target);
    }

    PlayerAction[] BuildActionsToward(PlayerMover bot, string targetCity)
    {
        var actions = new PlayerAction[4];
        string current = bot.currentCityName;

        for (int i = 0; i < 4; i++)
        {
            string bestColor = GetColorToward(current, targetCity);
            string next = bestColor != null ? bot.FindNextCityByColor(current, bestColor) : null;
            string dest = next ?? current;

            bool interact = MobObjectSpawner.chestCities.Contains(dest) ||
                            MobObjectSpawner.towerCities.Contains(dest) ||
                            MobObjectSpawner.buildingCities.Contains(dest);

            PlayerAction.PathColor? pathColor = null;
            if (bestColor != null && System.Enum.TryParse(bestColor, out PlayerAction.PathColor pc))
                pathColor = pc;

            actions[i] = new PlayerAction(pathColor, interact);
            if (next != null) current = next;
        }
        return actions;
    }

    // ── BFS Helpers ──────────────────────────────────────────────────────────

    string FindNearestChest(string startCity) =>
        BFSFind(startCity, city => MobObjectSpawner.chestCities.Contains(city));

    string FindNearestTower(string startCity) =>
        BFSFind(startCity, city => MobObjectSpawner.towerCities.Contains(city));

    string BFSFind(string startCity, System.Func<string, bool> predicate)
    {
        if (PathFinder.Instance == null) return null;

        var visited = new HashSet<string> { startCity };
        var queue   = new Queue<string>();
        queue.Enqueue(startCity);

        while (queue.Count > 0)
        {
            string city = queue.Dequeue();
            if (predicate(city) && city != startCity) return city;

            foreach (string color in Colors)
            {
                var neighbors = PathFinder.Instance.GetConnections(city, color);
                if (neighbors == null) continue;
                foreach (string neighbor in neighbors)
                {
                    if (visited.Add(neighbor))
                        queue.Enqueue(neighbor);
                }
            }
        }
        return null;
    }

    // BFS that tracks the first color taken, so we know which direction to step
    string GetColorToward(string fromCity, string targetCity)
    {
        if (PathFinder.Instance == null || fromCity == targetCity)
            return Colors[Random.Range(0, Colors.Length)];

        var visited = new HashSet<string> { fromCity };
        var queue   = new Queue<(string city, string firstColor)>();

        foreach (string color in Colors)
        {
            var neighbors = PathFinder.Instance.GetConnections(fromCity, color);
            if (neighbors == null) continue;
            foreach (string neighbor in neighbors)
            {
                if (visited.Add(neighbor))
                    queue.Enqueue((neighbor, color));
            }
        }

        while (queue.Count > 0)
        {
            var (city, firstColor) = queue.Dequeue();
            if (city == targetCity) return firstColor;

            foreach (string color in Colors)
            {
                var neighbors = PathFinder.Instance.GetConnections(city, color);
                if (neighbors == null) continue;
                foreach (string neighbor in neighbors)
                {
                    if (visited.Add(neighbor))
                        queue.Enqueue((neighbor, firstColor));
                }
            }
        }

        return Colors[Random.Range(0, Colors.Length)];
    }
}
