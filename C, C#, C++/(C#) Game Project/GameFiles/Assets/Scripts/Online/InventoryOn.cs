using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOn : MonoBehaviour
{
    public enum ItemType
    {
        Gold,
        Sword,
        Shield,
        Bow
    }

    private Dictionary<ItemType, int> items = new();

    public event Action OnInventoryChanged;

    void Awake()
    {
        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            items[type] = 0;
        }
    }

  
    public void ModifyItem(ItemType type, int amount)
    {
        if (!items.ContainsKey(type))
            items[type] = 0;

        items[type] += amount;
        items[type] = Mathf.Max(0, items[type]);

        Debug.Log($"[InventoryOn] {gameObject.name}: {type} {(amount >= 0 ? "+" : "")}{amount} → {items[type]}");

        OnInventoryChanged?.Invoke();
    }

    public int GetItemCount(ItemType type)
    {
        return items.TryGetValue(type, out int value) ? value : 0;
    }
    public int GetTotalItems()
    {
        return
            GetItemCount(ItemType.Sword) +
            GetItemCount(ItemType.Shield) +
            GetItemCount(ItemType.Bow);
    }

}
