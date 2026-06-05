using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; // 
    public TMP_Text[] slotCounters; // 0: Gold, 1: Sword, 2: Shield, 3: Bow

    public void Initialize(Inventory inv)
    {
        inventory = inv;
        inventory.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }

    void UpdateUI()
    {
        //Debug.Log("[InventoryUI] Updating display...");
        slotCounters[0].text = inventory.GetItemCount(Inventory.ItemType.Gold).ToString();
        slotCounters[1].text = inventory.GetItemCount(Inventory.ItemType.Sword).ToString();
        slotCounters[2].text = inventory.GetItemCount(Inventory.ItemType.Shield).ToString();
        slotCounters[3].text = inventory.GetItemCount(Inventory.ItemType.Bow).ToString();
    }
}
