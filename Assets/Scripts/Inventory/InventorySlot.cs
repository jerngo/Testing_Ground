using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int amount;

    public bool IsEmpty => item == null;
    public bool CanStack(ItemData other) => item != null && item.itemID == other.itemID && item.isStackable;

    public void Set(ItemData newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }

    public void AddAmount(int value) => amount += value;
}