using System.Collections.Generic;
using UnityEngine;

public enum BuildingType { Market, BanditDen, Shrine, WanderingGambler }

public class BuildingInteraction : MonoBehaviour
{
    public BuildingType buildingType;

    private int[] lastVisitedTurn = { -99, -99 };
    private int[] lastGamblerVisitTurn = { -99, -99 };
    private const int MarketCooldownTurns = 4;
    private const int GamblerCooldownTurns = 4;
    private const int MarketItemCost = 3;
    private const int MarketAbilityCost = 4;

    public void OnPlayerEnter(PlayerMover player, int playerIndex)
    {
        switch (buildingType)
        {
            case BuildingType.Market:
                HandleMarket(player, playerIndex);
                break;
            case BuildingType.BanditDen:
                HandleBanditDen(player, playerIndex);
                break;
            case BuildingType.Shrine:
                HandleShrine(player, playerIndex);
                break;
            case BuildingType.WanderingGambler:
                HandleWanderingGambler(player, playerIndex);
                break;
        }
    }

    void HandleMarket(PlayerMover player, int playerIndex)
    {
        int currentTurn = GameManager.Instance != null ? GameManager.Instance.TurnNumber : 0;
        int lastVisit = lastVisitedTurn[playerIndex];

        if (currentTurn - lastVisit < MarketCooldownTurns)
        {
            int turnsLeft = MarketCooldownTurns - (currentTurn - lastVisit);
            GameLog.Instance?.Add($"P{playerIndex+1} Market: {turnsLeft} turn(s) cooldown left");
            return;
        }

        Inventory inv = player.GetComponent<Inventory>();
        if (inv == null) return;

        if (BotAI.IsBotTurn)
        {
            BotAutoHandleMarket(inv, playerIndex);
            return;
        }

        MarketUI ui = MarketUI.Instance ?? FindUI<MarketUI>();
        if (ui == null) { Debug.LogError("[Market] MarketUI not found in scene!"); return; }
        ui.OpenMarket(inv, playerIndex, this);
        // NOTE: lastVisitedTurn is set only after a purchase via RegisterMarketVisit()
    }

    void BotAutoHandleMarket(Inventory inv, int playerIndex)
    {
        if (inv.GetItemCount(Inventory.ItemType.Gold) < MarketItemCost) return;
        var items = new[] { Inventory.ItemType.Sword, Inventory.ItemType.Shield, Inventory.ItemType.Bow };
        var item = items[Random.Range(0, items.Length)];
        inv.ModifyItem(Inventory.ItemType.Gold, -MarketItemCost);
        inv.ModifyItem(item, 1);
        RegisterMarketVisit(playerIndex);
        GameLog.Instance?.Add($"P{playerIndex+1} (Bot) bought 1 {item} from Market ({MarketItemCost}g)");
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.chest);
    }

    public void RegisterMarketVisit(int playerIndex)
    {
        int currentTurn = GameManager.Instance != null ? GameManager.Instance.TurnNumber : 0;
        lastVisitedTurn[playerIndex] = currentTurn;
    }

    public void BuyAbilityScroll(Inventory inv, AbilityManager abilityMgr, AbilityType abilityType, int playerIndex)
    {
        if (inv.GetItemCount(Inventory.ItemType.Gold) < MarketAbilityCost) return;
        inv.ModifyItem(Inventory.ItemType.Gold, -MarketAbilityCost);
        abilityMgr.GrantFreeAbilityCharge(abilityType);
        RegisterMarketVisit(playerIndex);
        GameLog.Instance?.Add($"P{playerIndex+1} bought {abilityType} scroll ({MarketAbilityCost}g)");
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.chest);
    }

    public void BuyItem(Inventory inv, Inventory.ItemType item, int playerIndex)
    {
        if (inv.GetItemCount(Inventory.ItemType.Gold) < MarketItemCost) return;
        inv.ModifyItem(Inventory.ItemType.Gold, -MarketItemCost);
        inv.ModifyItem(item, 1);
        RegisterMarketVisit(playerIndex);
        GameLog.Instance?.Add($"P{playerIndex+1} bought 1 {item} from Market ({MarketItemCost}g)");
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.chest);
    }

    void HandleBanditDen(PlayerMover player, int playerIndex)
    {
        Inventory inv = player.GetComponent<Inventory>();
        if (inv == null) return;

        var available = new List<Inventory.ItemType>();
        foreach (Inventory.ItemType t in System.Enum.GetValues(typeof(Inventory.ItemType)))
        {
            if (inv.GetItemCount(t) > 0)
                available.Add(t);
        }

        if (available.Count == 0)
        {
            GameLog.Instance?.Add($"P{playerIndex+1} hit Bandit Den — nothing to steal");
        }
        else
        {
            Inventory.ItemType stolen = available[Random.Range(0, available.Count)];
            inv.ModifyItem(stolen, -1);
            GameLog.Instance?.Add($"P{playerIndex+1} lost 1 {stolen} to Bandit Den!");
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.tower);
        // BanditDen stays — no DestroyBuilding
    }

    void HandleShrine(PlayerMover player, int playerIndex)
    {
        AbilityManager abilityMgr = player.GetComponent<AbilityManager>();
        if (abilityMgr != null)
        {
            var values = (AbilityType[])System.Enum.GetValues(typeof(AbilityType));
            AbilityType granted = values[Random.Range(0, values.Length)];
            abilityMgr.GrantFreeAbilityCharge(granted);
            GameLog.Instance?.Add($"P{playerIndex+1} visited Shrine — got {granted} charge!");
            AbilityUI.Instance?.RefreshUI();
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.chest);

        int respawnTurn = (GameManager.Instance != null ? GameManager.Instance.TurnNumber : 0) + 5;
        MobObjectSpawner.Instance?.ScheduleShrineRespawn(respawnTurn);
        DestroyBuilding();
    }

    void HandleWanderingGambler(PlayerMover player, int playerIndex)
    {
        int currentTurn = GameManager.Instance != null ? GameManager.Instance.TurnNumber : 0;
        int lastVisit = lastGamblerVisitTurn[playerIndex];

        if (currentTurn - lastVisit < GamblerCooldownTurns)
        {
            int turnsLeft = GamblerCooldownTurns - (currentTurn - lastVisit);
            GameLog.Instance?.Add($"P{playerIndex+1} Gambler: {turnsLeft} turn(s) cooldown left");
            return;
        }

        Inventory inv = player.GetComponent<Inventory>();
        if (inv == null) return;

        if (BotAI.IsBotTurn)
        {
            BotAutoHandleGambler(inv, playerIndex);
            return;
        }

        GamblerUI ui = GamblerUI.Instance ?? FindUI<GamblerUI>();
        if (ui == null) { Debug.LogError("[Gambler] GamblerUI not found in scene!"); return; }
        ui.OpenGambler(inv, playerIndex, this);
    }

    void BotAutoHandleGambler(Inventory inv, int playerIndex)
    {
        if (inv.GetItemCount(Inventory.ItemType.Gold) < 5) return;
        bool choseOdd = Random.value > 0.5f;
        int roll = Random.Range(1, 7);
        bool isOdd = roll % 2 != 0;
        bool won = choseOdd == isOdd;
        inv.ModifyItem(Inventory.ItemType.Gold, won ? 5 : -5);
        RegisterGamblerVisit(playerIndex);
        string result = won ? $"won +5g" : $"lost -5g";
        GameLog.Instance?.Add($"P{playerIndex+1} (Bot) gambled: rolled {roll}, {result}");
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(won ? AudioManager.Instance.chest : AudioManager.Instance.tower);
    }

    public void RegisterGamblerVisit(int playerIndex)
    {
        int currentTurn = GameManager.Instance != null ? GameManager.Instance.TurnNumber : 0;
        lastGamblerVisitTurn[playerIndex] = currentTurn;
    }

    static T FindUI<T>() where T : MonoBehaviour
    {
        var all = Resources.FindObjectsOfTypeAll<T>();
        if (all.Length == 0) return null;
        T found = all[0];
        found.gameObject.SetActive(true);
        return found;
    }

    void DestroyBuilding()
    {
        string cityName = transform.parent?.parent?.name.Replace("City_", "");
        if (!string.IsNullOrEmpty(cityName))
            MobObjectSpawner.buildingCities.Remove(cityName);

        Destroy(gameObject);
    }
}
