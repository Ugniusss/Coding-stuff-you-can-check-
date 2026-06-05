using System;
using System.Collections.Generic;
using UnityEngine;

// Reads path graph and board state from the Unity scene once, then runs
// fully pure-C# game simulations for MCTS rollouts.
// Not a MonoBehaviour — instantiate via BuildFromScene().
public class GameSimulator
{
    private readonly Dictionary<string, List<string>> _red    = new Dictionary<string, List<string>>();
    private readonly Dictionary<string, List<string>> _blue   = new Dictionary<string, List<string>>();
    private readonly Dictionary<string, List<string>> _yellow = new Dictionary<string, List<string>>();

    private static readonly string[]      Colors = { "Red", "Blue", "Yellow" };
    private static readonly System.Random Rng    = new System.Random();

    // ── Factory ───────────────────────────────────────────────────────────────

    // Call from BotAI.Start() after PathFinder.Awake() has already run.
    public static GameSimulator BuildFromScene()
    {
        var sim = new GameSimulator();
        sim.Init();
        return sim;
    }

    void Init()
    {
        PathFinder pf = PathFinder.Instance;
        if (pf == null) { Debug.LogError("[GameSimulator] PathFinder.Instance is null!"); return; }

        foreach (string city in CollectCityNames())
        {
            AddConnections(pf, city, "Red",    _red);
            AddConnections(pf, city, "Blue",   _blue);
            AddConnections(pf, city, "Yellow", _yellow);
        }
    }

    void AddConnections(PathFinder pf, string city, string color,
        Dictionary<string, List<string>> dest)
    {
        if (dest.ContainsKey(city)) return;
        List<string> conns = pf.GetConnections(city, color);
        if (conns != null && conns.Count > 0)
            dest[city] = new List<string>(conns);
    }

    static List<string> CollectCityNames()
    {
        var names = new List<string>();
        PlayerMover[] movers = UnityEngine.Object.FindObjectsOfType<PlayerMover>(true);
        if (movers.Length == 0) return names;
        Transform cp = movers[0].citiesParent;
        if (cp == null) return names;
        foreach (Transform city in cp)
            names.Add(city.name.Replace("City_", ""));
        return names;
    }

    // ── Scene Snapshot ────────────────────────────────────────────────────────

    // Must be called from main thread.
    public GameStateSim SnapshotCurrentState()
    {
        var state = new GameStateSim
        {
            currentPlayerIndex = GameManager.Instance?.currentPlayerIndex ?? 1,
            roundsLeft         = Math.Max(1, 15 - (GameManager.Instance?.TurnNumber ?? 1)),
            towerCities        = new HashSet<string>(MobObjectSpawner.towerCities),
            chestCities        = new HashSet<string>(MobObjectSpawner.chestCities),
            buildingMap        = new Dictionary<string, BuildingType>(),
        };

        foreach (BuildingInteraction bi in UnityEngine.Object.FindObjectsOfType<BuildingInteraction>())
        {
            Transform ct = bi.transform.parent?.parent;
            if (ct == null) continue;
            string city = ct.name.Replace("City_", "");
            if (!string.IsNullOrEmpty(city) && !state.buildingMap.ContainsKey(city))
                state.buildingMap[city] = bi.buildingType;
        }

        PlayerMover[] pms = GameManager.Instance?.players;
        if (pms != null)
            for (int i = 0; i < 2 && i < pms.Length; i++)
                state.players[i] = SnapshotPlayer(pms[i]);

        return state;
    }

    static GameStateSim.PlayerSim SnapshotPlayer(PlayerMover pm)
    {
        Inventory      inv = pm.GetComponent<Inventory>();
        AbilityManager am  = pm.GetComponent<AbilityManager>();

        var charges = new int[4];
        if (am != null)
            foreach (AbilityType t in Enum.GetValues(typeof(AbilityType)))
                charges[(int)t] = am.GetChargeCount(t);

        return new GameStateSim.PlayerSim
        {
            cityName         = pm.currentCityName,
            prevCity         = null, // private in PlayerMover — acceptable approximation
            playerClass      = pm.playerClass,
            gold             = inv?.GetItemCount(Inventory.ItemType.Gold)   ?? 0,
            sword            = inv?.GetItemCount(Inventory.ItemType.Sword)  ?? 0,
            shield           = inv?.GetItemCount(Inventory.ItemType.Shield) ?? 0,
            bow              = inv?.GetItemCount(Inventory.ItemType.Bow)    ?? 0,
            abilityCharges   = charges,
            fortifyTurnsLeft = (am != null && am.fortified) ? 1 : 0,
            isAmbushed       = am?.ambushed ?? false,
        };
    }

    // ── Candidate Actions ─────────────────────────────────────────────────────

    public List<BotAction> GetCandidateActions(GameStateSim state)
    {
        if (state.roundsLeft <= 0) return new List<BotAction> { EmptyAction() };

        int pIdx = state.currentPlayerIndex;
        var p    = state.players[pIdx];

        var abilityOpts = new List<AbilityType?> { null };
        foreach (AbilityType t in Enum.GetValues(typeof(AbilityType)))
            if (p.abilityCharges[(int)t] > 0)
                abilityOpts.Add(t);

        var seqs = BuildMoveSequences(state, pIdx);

        var actions = new List<BotAction>(abilityOpts.Count * seqs.Count);
        foreach (var ability in abilityOpts)
            foreach (var seq in seqs)
                actions.Add(new BotAction { ability = ability, moves = seq.m, doActions = seq.d });

        if (actions.Count == 0) actions.Add(EmptyAction());
        return actions;
    }

    List<(string[] m, bool[] d)> BuildMoveSequences(GameStateSim state, int pIdx)
    {
        var result  = new List<(string[], bool[])>();
        var p       = state.players[pIdx];

        // Prioritised targets
        int thr = ClassData.TowerThreshold(p.playerClass);
        if (p.sword >= thr && p.shield >= thr && p.bow >= thr)
        {
            string t = NearestIn(p.cityName, state.towerCities);
            if (t != null) result.Add(SeqToward(p.cityName, p.prevCity, t, state));
        }

        string chest = NearestIn(p.cityName, state.chestCities);
        if (chest != null) result.Add(SeqToward(p.cityName, p.prevCity, chest, state));

        foreach (string building in state.buildingMap.Keys)
            result.Add(SeqToward(p.cityName, p.prevCity, building, state));

        // Random exploration sequences
        for (int i = 0; i < 3; i++)
            result.Add(RandomSeq(p.cityName, p.prevCity, state));

        return result;
    }

    (string[] m, bool[] d) SeqToward(string start, string prev, string target, GameStateSim state)
    {
        string cur = start, prevC = prev;
        var moves = new string[4];
        var doAct = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            string color = ColorToward(cur, prevC, target);
            if (color == null) continue;
            string next = FindNextCity(cur, prevC, color);
            moves[i] = color;
            if (next != null)
            {
                doAct[i] = IsValuable(next, state);
                prevC = cur; cur = next;
            }
        }
        return (moves, doAct);
    }

    (string[] m, bool[] d) RandomSeq(string start, string prev, GameStateSim state)
    {
        string cur = start, prevC = prev;
        var moves = new string[4];
        var doAct = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            var avail = AvailableColors(cur);
            if (avail.Count == 0) continue;
            string color = avail[Rng.Next(avail.Count)];
            string next  = FindNextCity(cur, prevC, color);
            moves[i] = color;
            if (next != null)
            {
                doAct[i] = IsValuable(next, state);
                prevC = cur; cur = next;
            }
        }
        return (moves, doAct);
    }

    // Helper for MCTSBot.Expand — opponent's random turn.
    public (string[] m, bool[] d) GetRandomMoves(GameStateSim state)
        => RandomSeq(state.players[state.currentPlayerIndex].cityName,
                     state.players[state.currentPlayerIndex].prevCity, state);

    static BotAction EmptyAction() => new BotAction
        { ability = null, moves = new string[4], doActions = new bool[4] };

    // ── Action Application (pure, mutates state) ──────────────────────────────

    public void ApplyAction(GameStateSim state, BotAction action)
    {
        int pIdx   = state.currentPlayerIndex;
        int oppIdx = 1 - pIdx;

        // Ability phase
        state.players[pIdx].isAmbushed = false;
        if (state.players[pIdx].fortifyTurnsLeft > 0)
            state.players[pIdx].fortifyTurnsLeft--;

        if (action.ability.HasValue)
        {
            int aIdx = (int)action.ability.Value;
            if (state.players[pIdx].abilityCharges[aIdx] > 0)
            {
                state.players[pIdx].abilityCharges[aIdx]--;
                SimAbility(state, pIdx, action.ability.Value);
            }
        }

        // Move phase
        if (!state.players[pIdx].isAmbushed)
        {
            for (int i = 0; i < 4; i++)
            {
                string color = (action.moves   != null && i < action.moves.Length)   ? action.moves[i]   : null;
                bool   doAct = (action.doActions != null && i < action.doActions.Length) && action.doActions[i];

                if (color != null)
                {
                    string next = FindNextCity(state.players[pIdx].cityName,
                                              state.players[pIdx].prevCity, color);
                    if (next != null)
                    {
                        state.players[pIdx].prevCity = state.players[pIdx].cityName;
                        state.players[pIdx].cityName = next;
                    }
                }

                SimBanditDen(state, pIdx);
                if (doAct) { SimChest(state, pIdx); SimTower(state, pIdx); SimBuilding(state, pIdx); }
            }
        }

        state.currentPlayerIndex = oppIdx;
        if (pIdx == 1) state.roundsLeft--;
    }

    void SimAbility(GameStateSim state, int pIdx, AbilityType type)
    {
        int oIdx = 1 - pIdx;

        switch (type)
        {
            case AbilityType.Bribe:
                if (state.players[oIdx].fortifyTurnsLeft > 0) break;
                if (state.players[oIdx].gold > 0)
                {
                    int stolen = Math.Max(1, state.players[oIdx].gold * 2 / 5);
                    state.players[oIdx].gold  = Math.Max(0, state.players[oIdx].gold - stolen);
                    state.players[pIdx].gold += stolen;
                }
                else
                {
                    if      (state.players[oIdx].sword  > 0) { state.players[oIdx].sword--;  state.players[pIdx].sword++;  }
                    else if (state.players[oIdx].shield > 0) { state.players[oIdx].shield--; state.players[pIdx].shield++; }
                    else if (state.players[oIdx].bow    > 0) { state.players[oIdx].bow--;    state.players[pIdx].bow++;    }
                }
                break;

            case AbilityType.Ambush:
                if (state.players[oIdx].fortifyTurnsLeft <= 0)
                    state.players[oIdx].isAmbushed = true;
                break;

            case AbilityType.Fortify:
                state.players[pIdx].fortifyTurnsLeft = 3;
                break;

            case AbilityType.Teleport:
                string dest = NearestIn(state.players[pIdx].cityName, state.chestCities)
                           ?? NearestIn(state.players[pIdx].cityName, state.towerCities);
                if (dest != null)
                {
                    state.players[pIdx].prevCity = state.players[pIdx].cityName;
                    state.players[pIdx].cityName = dest;
                }
                break;
        }
    }

    void SimBanditDen(GameStateSim state, int pIdx)
    {
        string city = state.players[pIdx].cityName;
        if (!state.buildingMap.TryGetValue(city, out BuildingType bt) || bt != BuildingType.BanditDen) return;
        if (ClassData.BanditDenImmune(state.players[pIdx].playerClass)) return;

        if      (state.players[pIdx].gold   > 0) state.players[pIdx].gold--;
        else if (state.players[pIdx].sword  > 0) state.players[pIdx].sword--;
        else if (state.players[pIdx].shield > 0) state.players[pIdx].shield--;
        else if (state.players[pIdx].bow    > 0) state.players[pIdx].bow--;
    }

    void SimChest(GameStateSim state, int pIdx)
    {
        if (!state.chestCities.Contains(state.players[pIdx].cityName)) return;
        state.chestCities.Remove(state.players[pIdx].cityName);
        state.players[pIdx].gold++; // simplified expected value
    }

    void SimTower(GameStateSim state, int pIdx)
    {
        string city = state.players[pIdx].cityName;
        if (!state.towerCities.Contains(city)) return;

        int thr = ClassData.TowerThreshold(state.players[pIdx].playerClass);
        if (state.players[pIdx].sword  >= thr &&
            state.players[pIdx].shield >= thr &&
            state.players[pIdx].bow    >= thr)
        {
            state.players[pIdx].sword  -= thr;
            state.players[pIdx].shield -= thr;
            state.players[pIdx].bow    -= thr;
            state.players[pIdx].gold   += 10;
            state.towerCities.Remove(city);
        }
        else
        {
            state.players[pIdx].gold = Math.Max(0, state.players[pIdx].gold - 5);
        }
    }

    void SimBuilding(GameStateSim state, int pIdx)
    {
        string city = state.players[pIdx].cityName;
        if (!state.buildingMap.TryGetValue(city, out BuildingType bt)) return;

        switch (bt)
        {
            case BuildingType.Market:
                if (state.players[pIdx].gold >= 3)
                {
                    state.players[pIdx].gold -= 3;
                    state.players[pIdx].sword++;
                }
                break;
            case BuildingType.Shrine:
                state.players[pIdx].abilityCharges[(int)AbilityType.Bribe]++;
                state.buildingMap.Remove(city);
                break;
        }
    }

    // ── Rollout ───────────────────────────────────────────────────────────────

    // Simulates game to completion with random play. Returns winning player index.
    public int Rollout(GameStateSim state, int botIndex)
    {
        var sim = state.Clone();

        while (sim.roundsLeft > 0)
        {
            var (m, d) = RandomSeq(sim.players[sim.currentPlayerIndex].cityName,
                                   sim.players[sim.currentPlayerIndex].prevCity, sim);
            ApplyAction(sim, new BotAction { ability = null, moves = m, doActions = d });
        }

        return sim.Winner();
    }

    // ── Path Helpers ──────────────────────────────────────────────────────────

    public string FindNextCity(string from, string prev, string color)
    {
        var map = GetMap(color);
        if (!map.TryGetValue(from, out var conns) || conns.Count == 0) return null;
        foreach (string c in conns)
            if (c != prev) return c;
        return conns[0]; // backtrack allowed as fallback
    }

    public string ColorToward(string from, string prev, string target)
    {
        if (from == target) return null;

        var visited = new HashSet<string> { from };
        var queue   = new Queue<(string city, string firstColor)>();

        foreach (string color in Colors)
        {
            if (!GetMap(color).TryGetValue(from, out var conns)) continue;
            foreach (string nb in conns)
                if (visited.Add(nb)) queue.Enqueue((nb, color));
        }

        while (queue.Count > 0)
        {
            var (city, fc) = queue.Dequeue();
            if (city == target) return fc;

            foreach (string color in Colors)
            {
                if (!GetMap(color).TryGetValue(city, out var conns)) continue;
                foreach (string nb in conns)
                    if (visited.Add(nb)) queue.Enqueue((nb, fc));
            }
        }
        return null;
    }

    public string NearestIn(string from, IEnumerable<string> targets)
    {
        var set = targets as HashSet<string> ?? new HashSet<string>(targets);
        if (set.Count == 0) return null;

        var visited = new HashSet<string> { from };
        var queue   = new Queue<string>();
        queue.Enqueue(from);

        while (queue.Count > 0)
        {
            string city = queue.Dequeue();
            if (set.Contains(city) && city != from) return city;

            foreach (string color in Colors)
            {
                if (!GetMap(color).TryGetValue(city, out var conns)) continue;
                foreach (string nb in conns)
                    if (visited.Add(nb)) queue.Enqueue(nb);
            }
        }
        return null;
    }

    List<string> AvailableColors(string city)
    {
        var result = new List<string>(3);
        if (_red.ContainsKey(city))    result.Add("Red");
        if (_blue.ContainsKey(city))   result.Add("Blue");
        if (_yellow.ContainsKey(city)) result.Add("Yellow");
        return result;
    }

    public bool IsValuable(string city, GameStateSim state)
        => state.chestCities.Contains(city) ||
           state.towerCities.Contains(city) ||
           state.buildingMap.ContainsKey(city);

    public int BFSDistance(string from, IEnumerable<string> targets)
    {
        var set = targets as HashSet<string> ?? new HashSet<string>(targets);
        if (set.Count == 0) return 99;
        if (set.Contains(from)) return 0;

        var visited = new HashSet<string> { from };
        var queue   = new Queue<(string city, int dist)>();
        queue.Enqueue((from, 0));

        while (queue.Count > 0)
        {
            var (city, dist) = queue.Dequeue();
            foreach (string color in Colors)
            {
                if (!GetMap(color).TryGetValue(city, out var conns)) continue;
                foreach (string nb in conns)
                {
                    if (!visited.Add(nb)) continue;
                    if (set.Contains(nb)) return dist + 1;
                    queue.Enqueue((nb, dist + 1));
                }
            }
        }
        return 99;
    }

    public List<string> GetAllCities()
    {
        var all = new HashSet<string>();
        foreach (var k in _red.Keys)    all.Add(k);
        foreach (var k in _blue.Keys)   all.Add(k);
        foreach (var k in _yellow.Keys) all.Add(k);
        return new List<string>(all);
    }

    Dictionary<string, List<string>> GetMap(string color)
        => color == "Red" ? _red : color == "Blue" ? _blue : _yellow;
}
