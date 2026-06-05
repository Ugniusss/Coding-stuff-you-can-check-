using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

// ML-Agents Agent for Hard difficulty bot.
// BehaviorParameters (set in Inspector):
//   Vector Obs Size : 22
//   Discrete Actions: 5 branches — sizes [5, 4, 4, 4, 4]
//   Behavior Name   : "HardBot"
public class BotAgent : Agent
{
    // Set by BotTrainingManager each turn before RequestDecision().
    // Also set by BotAI.cs during inference in OfflineBot.
    [HideInInspector] public BotTrainingManager manager;
    [HideInInspector] public GameStateSim        obsState;
    [HideInInspector] public GameSimulator       sim;

    // Inference handshake (used by BotAI coroutine)
    [HideInInspector] public bool      actionReady;
    [HideInInspector] public BotAction lastAction;

    private static readonly string[] Colors = { "Red", "Blue", "Yellow" };

    public override void OnEpisodeBegin()
    {
        actionReady = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (obsState == null || sim == null)
        {
            // Push zeros so obs size stays fixed
            for (int i = 0; i < 22; i++) sensor.AddObservation(0f);
            return;
        }

        var me  = obsState.players[0];
        var opp = obsState.players[1];

        sensor.AddObservation(obsState.roundsLeft / 15f);

        sensor.AddObservation(Mathf.Clamp01(me.gold  / 30f));
        sensor.AddObservation(Mathf.Clamp01(opp.gold / 30f));

        sensor.AddObservation(Mathf.Clamp01(me.sword   / 5f));
        sensor.AddObservation(Mathf.Clamp01(me.shield  / 5f));
        sensor.AddObservation(Mathf.Clamp01(me.bow     / 5f));
        sensor.AddObservation(Mathf.Clamp01(opp.sword  / 5f));
        sensor.AddObservation(Mathf.Clamp01(opp.shield / 5f));
        sensor.AddObservation(Mathf.Clamp01(opp.bow    / 5f));

        for (int i = 0; i < 4; i++) sensor.AddObservation(Mathf.Clamp01(me.abilityCharges[i]  / 3f));
        for (int i = 0; i < 4; i++) sensor.AddObservation(Mathf.Clamp01(opp.abilityCharges[i] / 3f));

        sensor.AddObservation(Mathf.Clamp01(me.fortifyTurnsLeft  / 3f));
        sensor.AddObservation(Mathf.Clamp01(opp.fortifyTurnsLeft / 3f));

        sensor.AddObservation(Mathf.Clamp01(sim.BFSDistance(me.cityName, obsState.chestCities) / 10f));
        sensor.AddObservation(Mathf.Clamp01(sim.BFSDistance(me.cityName, obsState.towerCities) / 10f));
        sensor.AddObservation(Mathf.Clamp01(sim.BFSDistance(me.cityName, ShrineCities(obsState)) / 10f));
        // 1+2+6+8+2+3 = 22 total
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var da = actions.DiscreteActions;

        AbilityType? ability = da[0] > 0 ? (AbilityType?)(AbilityType)(da[0] - 1) : null;

        var moves  = new string[4];
        var doActs = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            int c = da[1 + i];
            moves[i] = c > 0 ? Colors[c - 1] : null;
        }

        // Derive doAction: interact if destination city has something valuable
        if (obsState != null && sim != null)
        {
            string cur = obsState.players[0].cityName;
            string prev = obsState.players[0].prevCity;
            for (int i = 0; i < 4; i++)
            {
                if (moves[i] != null)
                {
                    string next = sim.FindNextCity(cur, prev, moves[i]);
                    if (next != null) { prev = cur; cur = next; }
                }
                doActs[i] = sim.IsValuable(cur, obsState);
            }
        }

        lastAction  = new BotAction { ability = ability, moves = moves, doActions = doActs };
        actionReady = true;

        // During training: notify manager to apply the action and advance game
        manager?.OnAgentActed(lastAction);
    }

    // Simple heuristic used for testing without Python (Editor play mode, no training).
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var da = actionsOut.DiscreteActions;

        // No ability by default
        da[0] = 0;

        if (obsState == null || sim == null) return;

        // Use Bribe if we have it and opponent is ahead
        var me  = obsState.players[0];
        var opp = obsState.players[1];
        if (me.abilityCharges[(int)AbilityType.Bribe] > 0 && opp.gold > me.gold + 3)
            da[0] = (int)AbilityType.Bribe + 1;

        // Navigate toward nearest chest
        string target = sim.NearestIn(me.cityName, obsState.chestCities)
                     ?? sim.NearestIn(me.cityName, obsState.towerCities);

        string cur = me.cityName, prev = me.prevCity;
        for (int i = 0; i < 4; i++)
        {
            string color = target != null
                ? sim.ColorToward(cur, prev, target)
                : Colors[Random.Range(0, Colors.Length)];

            da[1 + i] = color == "Red" ? 1 : color == "Blue" ? 2 : color == "Yellow" ? 3 : 0;

            if (color != null)
            {
                string next = sim.FindNextCity(cur, prev, color);
                if (next != null) { prev = cur; cur = next; }
            }
        }
    }

    static HashSet<string> ShrineCities(GameStateSim state)
    {
        var result = new HashSet<string>();
        foreach (var kv in state.buildingMap)
            if (kv.Value == BuildingType.Shrine) result.Add(kv.Key);
        return result;
    }
}
