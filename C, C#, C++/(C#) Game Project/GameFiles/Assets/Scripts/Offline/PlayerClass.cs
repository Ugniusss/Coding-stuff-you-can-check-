public enum PlayerClass { Merchant, Warrior, Wanderer }

public static class ClassData
{
    public static int StartingGold(PlayerClass c) => c == PlayerClass.Merchant ? 3 : 0;
    public static bool StartsWithWeapons(PlayerClass c) => c == PlayerClass.Warrior;
    public static int TowerThreshold(PlayerClass c) => c == PlayerClass.Warrior ? 2 : 3;
    public static bool BanditDenImmune(PlayerClass c) => c == PlayerClass.Wanderer;
    public static int AbilityCostBonus(PlayerClass c) => c == PlayerClass.Merchant ? -1 : 0;
}
