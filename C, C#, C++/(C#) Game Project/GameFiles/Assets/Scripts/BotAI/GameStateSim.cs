using System.Collections.Generic;

// Action the bot commits to for one full turn.
public struct BotAction
{
    public AbilityType? ability;
    public string[]     moves;     // length 4 — "Red"/"Blue"/"Yellow"/null
    public bool[]       doActions; // length 4
}

// Pure C# game state used by GameSimulator and MCTSBot.
// No UnityEngine dependency — safe to clone and mutate on any thread.
public class GameStateSim
{
    public struct PlayerSim
    {
        public string      cityName;
        public string      prevCity;
        public PlayerClass playerClass;
        public int         gold, sword, shield, bow;
        public int[]       abilityCharges; // indexed by (int)AbilityType
        public int         fortifyTurnsLeft;
        public bool        isAmbushed;
    }

    public PlayerSim[] players = new PlayerSim[2];
    public int         currentPlayerIndex;
    public int         roundsLeft; // decrements after both players complete a turn

    public HashSet<string>                 towerCities  = new HashSet<string>();
    public HashSet<string>                 chestCities  = new HashSet<string>();
    public Dictionary<string, BuildingType> buildingMap = new Dictionary<string, BuildingType>();

    public GameStateSim Clone()
    {
        var c = new GameStateSim
        {
            currentPlayerIndex = currentPlayerIndex,
            roundsLeft         = roundsLeft,
            towerCities        = new HashSet<string>(towerCities),
            chestCities        = new HashSet<string>(chestCities),
            buildingMap        = new Dictionary<string, BuildingType>(buildingMap),
        };
        c.players[0] = ClonePlayer(players[0]);
        c.players[1] = ClonePlayer(players[1]);
        return c;
    }

    static PlayerSim ClonePlayer(PlayerSim p) => new PlayerSim
    {
        cityName         = p.cityName,
        prevCity         = p.prevCity,
        playerClass      = p.playerClass,
        gold             = p.gold,
        sword            = p.sword,
        shield           = p.shield,
        bow              = p.bow,
        abilityCharges   = (int[])p.abilityCharges.Clone(),
        fortifyTurnsLeft = p.fortifyTurnsLeft,
        isAmbushed       = p.isAmbushed,
    };

    // Heuristic score from botIndex's perspective.
    public float Evaluate(int botIndex)
    {
        int opp = 1 - botIndex;
        float goldDiff = players[botIndex].gold  - players[opp].gold;
        float itemDiff = (players[botIndex].sword  + players[botIndex].shield  + players[botIndex].bow)
                       - (players[opp].sword       + players[opp].shield       + players[opp].bow);
        return goldDiff + 0.5f * itemDiff;
    }

    // Returns winning player index, or -1 for tie.
    public int Winner()
    {
        int g0 = players[0].gold, g1 = players[1].gold;
        if (g0 != g1) return g0 > g1 ? 0 : 1;
        int i0 = players[0].sword + players[0].shield + players[0].bow;
        int i1 = players[1].sword + players[1].shield + players[1].bow;
        if (i0 != i1) return i0 > i1 ? 0 : 1;
        return -1;
    }
}
