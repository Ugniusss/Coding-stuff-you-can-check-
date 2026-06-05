using System.Collections.Generic;
using UnityEngine;

public enum AbilityType { Bribe, Ambush, Teleport, Fortify }

public class AbilityManager : MonoBehaviour
{
    private int fortifyTurnsLeft = 0;
    public bool fortified => fortifyTurnsLeft > 0;
    public bool ambushed  { get; private set; } = false;

    private Dictionary<AbilityType, int> charges = new();

    public int GetChargeCount(AbilityType type) => charges.TryGetValue(type, out int c) ? c : 0;
    public bool HasCharge(AbilityType type) => GetChargeCount(type) > 0;

    public void GrantFreeAbilityCharge()
    {
        var values = (AbilityType[])System.Enum.GetValues(typeof(AbilityType));
        GrantFreeAbilityCharge(values[Random.Range(0, values.Length)]);
    }

    public void GrantFreeAbilityCharge(AbilityType type)
    {
        charges[type] = GetChargeCount(type) + 1;
        Debug.Log($"[AbilityManager] +1 charge for {type}. Total: {charges[type]}");
    }

    public void ConsumeCharge(AbilityType type)
    {
        if (charges.TryGetValue(type, out int c) && c > 0) charges[type] = c - 1;
    }

    public void OnTurnStart()
    {
        ambushed = false;
        if (fortifyTurnsLeft > 0) fortifyTurnsLeft--;
    }

    public bool UseAbility(AbilityType type, PlayerMover self, PlayerMover opponent)
    {
        if (!HasCharge(type))
        {
            Debug.Log($"[Ability] No charges for {type}.");
            return false;
        }

        charges[type]--;

        switch (type)
        {
            case AbilityType.Bribe:    DoBribe(self, opponent); break;
            case AbilityType.Ambush:   DoAmbush(opponent);      break;
            case AbilityType.Teleport: DoTeleport(self);        break;
            case AbilityType.Fortify:  DoFortify();             break;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.chest);

        int pIdx = System.Array.IndexOf(GameManager.Instance?.players, self);
        string pName = pIdx >= 0 ? $"P{pIdx+1}" : "P?";
        GameLog.Instance?.Add($"{pName} used {type} (charges left: {charges[type]})");

        return true;
    }

    void DoBribe(PlayerMover self, PlayerMover opponent)
    {
        AbilityManager oppMgr = opponent.GetComponent<AbilityManager>();
        if (oppMgr != null && oppMgr.fortified)
        {
            Debug.Log("[Bribe] Opponent is fortified — blocked.");
            return;
        }

        Inventory selfInv = self.GetComponent<Inventory>();
        Inventory oppInv  = opponent.GetComponent<Inventory>();
        if (selfInv == null || oppInv == null) return;

        int oppGold = oppInv.GetItemCount(Inventory.ItemType.Gold);
        if (oppGold == 0)
        {
            var allItems = new List<Inventory.ItemType> {
                Inventory.ItemType.Sword, Inventory.ItemType.Shield, Inventory.ItemType.Bow
            };
            var available = allItems.FindAll(i => oppInv.GetItemCount(i) > 0);
            if (available.Count > 0)
            {
                var stolen = available[Random.Range(0, available.Count)];
                oppInv.ModifyItem(stolen, -1);
                selfInv.ModifyItem(stolen, 1);
            }
            return;
        }

        float pct = Random.Range(0.3f, 0.5f);
        int stolen2 = Mathf.Max(1, Mathf.FloorToInt(oppGold * pct));
        oppInv.ModifyItem(Inventory.ItemType.Gold, -stolen2);
        selfInv.ModifyItem(Inventory.ItemType.Gold, stolen2);
    }

    void DoAmbush(PlayerMover opponent)
    {
        AbilityManager oppMgr = opponent.GetComponent<AbilityManager>();
        if (oppMgr == null) return;
        if (oppMgr.fortified) return;
        oppMgr.ambushed = true;
    }

    void DoTeleport(PlayerMover self)
    {
        TeleportUI ui = TeleportUI.Instance;
        if (ui == null)
        {
            var all = Resources.FindObjectsOfTypeAll<TeleportUI>();
            if (all.Length > 0) { ui = all[0]; ui.gameObject.SetActive(true); }
        }
        if (ui == null) { Debug.LogError("[Teleport] TeleportUI not found in scene!"); return; }
        ui.OpenPicker(self);
    }

    void DoFortify()
    {
        fortifyTurnsLeft = 3;
    }
}
