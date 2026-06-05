using System.Collections.Generic;
using UnityEngine;

// Drives the ML-Agents training episode loop for the Hard bot.
// Attach to a GameObject in the BotTraining scene.
// BotAgent (learner) is always player 0; MCTS opponent is player 1.
public class BotTrainingManager : MonoBehaviour
{
    public BotAgent learner;   // drag Player_1 here (has BotAgent component)

    private GameSimulator  _sim;
    private GameStateSim   _state;
    private bool           _waitingForAgent;
    private List<string>   _allCities;
    private static System.Random _rng = new System.Random();

    void Start()
    {
        Time.timeScale = 20f;
        _sim       = GameSimulator.BuildFromScene();
        _allCities = _sim.GetAllCities();

        learner.manager = this;
        learner.sim     = _sim;

        _state = BuildInitialState();
        Debug.Log($"[BotTraining] Started. Cities: {_allCities.Count}, Rounds: {_state.roundsLeft}");
    }

    void FixedUpdate()
    {
        if (_waitingForAgent) return;
        if (_state == null)   return;

        if (_state.roundsLeft <= 0)
        {
            FinishEpisode();
            return;
        }

        if (_state.currentPlayerIndex == 0)
        {
            // Agent's turn — hand state to agent and request a decision
            _waitingForAgent   = true;
            learner.obsState   = _state.Clone(); // clone so agent reads stable snapshot
            learner.actionReady = false;
            learner.RequestDecision();
        }
        else
        {
            // Opponent's turn — MCTS with small budget so training doesn't stall
            BotAction oppAction = MCTSBot.GetBotAction(_state.Clone(), _sim, budgetMs: 5);
            _sim.ApplyAction(_state, oppAction);

            if (_state.roundsLeft <= 0)
                FinishEpisode();
        }
    }

    // Called by BotAgent.OnActionReceived during training
    public void OnAgentActed(BotAction action)
    {
        _sim.ApplyAction(_state, action);
        _waitingForAgent = false;

        if (_state.roundsLeft <= 0)
            FinishEpisode();
    }

    void FinishEpisode()
    {
        int winner = _state.Winner();
        // +1 agent wins, -1 agent loses, 0 tie
        float reward = winner == 0 ? 1f : winner == 1 ? -1f : 0f;
        Debug.Log($"[BotTraining] Episode done. Winner: {(winner == 0 ? "Agent" : winner == 1 ? "MCTS" : "Tie")}, Reward: {reward}");
        learner.SetReward(reward);
        learner.EndEpisode(); // → OnEpisodeBegin is called by ML-Agents

        _state           = BuildInitialState();
        _waitingForAgent = false;
    }

    GameStateSim BuildInitialState()
    {
        int n = _allCities.Count;
        var shuffled = Shuffle(_allCities);

        // Spread chests / towers / buildings across distinct city slots
        var towerCities  = new HashSet<string>();
        var chestCities  = new HashSet<string>();
        var buildingMap  = new Dictionary<string, BuildingType>();

        for (int i = 0;      i < 5  && i < n; i++) towerCities.Add(shuffled[i]);
        for (int i = 5;      i < 15 && i < n; i++) chestCities.Add(shuffled[i]);

        var bTypes = new[] { BuildingType.Market, BuildingType.BanditDen, BuildingType.Shrine, BuildingType.WanderingGambler };
        for (int i = 15, b = 0; i < n && b < bTypes.Length; i++, b++)
            buildingMap[shuffled[i]] = bTypes[b];

        var classes = (PlayerClass[])System.Enum.GetValues(typeof(PlayerClass));
        PlayerClass pc0 = classes[_rng.Next(classes.Length)];
        PlayerClass pc1 = classes[_rng.Next(classes.Length)];

        var state = new GameStateSim
        {
            currentPlayerIndex = 0,
            roundsLeft         = 15,
            towerCities        = towerCities,
            chestCities        = chestCities,
            buildingMap        = buildingMap,
        };
        state.players[0] = MakePlayer(pc0, shuffled[_rng.Next(n)]);
        state.players[1] = MakePlayer(pc1, shuffled[_rng.Next(n)]);
        return state;
    }

    static GameStateSim.PlayerSim MakePlayer(PlayerClass cls, string city)
    {
        int gold = ClassData.StartingGold(cls);
        int sword = 0, shield = 0, bow = 0;
        if (ClassData.StartsWithWeapons(cls)) { sword = shield = bow = 1; }

        return new GameStateSim.PlayerSim
        {
            cityName         = city,
            prevCity         = null,
            playerClass      = cls,
            gold             = gold,
            sword            = sword,
            shield           = shield,
            bow              = bow,
            abilityCharges   = new int[4],
            fortifyTurnsLeft = 0,
            isAmbushed       = false,
        };
    }

    static List<string> Shuffle(List<string> list)
    {
        var copy = new List<string>(list);
        for (int i = copy.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (copy[i], copy[j]) = (copy[j], copy[i]);
        }
        return copy;
    }
}
