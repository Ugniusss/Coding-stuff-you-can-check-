using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public enum ItemType { Gold, Sword, Shield, Bow }

    private Dictionary<ItemType, int> itemCounts = new();
    public event Action OnInventoryChanged;
    void Awake()
    {
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            itemCounts[type] = 0;
        }
    }

    public void ModifyItem(ItemType item, int amount)
    {
        if (!itemCounts.ContainsKey(item))
            itemCounts[item] = 0;

        itemCounts[item] += amount;
        itemCounts[item] = Mathf.Max(0, itemCounts[item]);

        Debug.Log($"[{gameObject.name}] Inventory updated: {item} {(amount > 0 ? "+" : "")}{amount}. Total: {itemCounts[item]}");

        OnInventoryChanged?.Invoke();
    }

    public int GetItemCount(ItemType item)
    {
        return itemCounts.ContainsKey(item) ? itemCounts[item] : 0;
    }
}
