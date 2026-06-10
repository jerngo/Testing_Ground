using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Settings")]
    public int slotCount = 24;

    public List<InventorySlot> slots = new();

    public event System.Action OnInventoryChanged;

    void Awake()
    {
        Instance = this;
        InitSlots();
    }

    void InitSlots()
    {
        slots.Clear();
        for (int i = 0; i < slotCount; i++)
            slots.Add(new InventorySlot());
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null) return false;

        // Coba stack dulu
        if (item.isStackable)
        {
            var existing = slots.FirstOrDefault(s => s.CanStack(item));
            if (existing != null)
            {
                existing.AddAmount(amount);
                QuestManager.Instance?.ReportCollect(item.itemID, amount);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // Cari slot kosong
        var empty = slots.FirstOrDefault(s => s.IsEmpty);
        if (empty == null)
        {
            Debug.Log("Inventory penuh!");
            return false;
        }

        empty.Set(item, amount);
        QuestManager.Instance?.ReportCollect(item.itemID, amount);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        var slot = slots.FirstOrDefault(s => s.item == item);
        if (slot == null) return false;

        slot.amount -= amount;
        if (slot.amount <= 0) slot.Clear();

        OnInventoryChanged?.Invoke();
        return true;
    }

    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA == indexB) return;
        if (indexA < 0 || indexB < 0) return;
        if (indexA >= slots.Count || indexB >= slots.Count) return;

        var slotA = slots[indexA];
        var slotB = slots[indexB];

        // Kalau sama item dan stackable — merge
        if (!slotA.IsEmpty && !slotB.IsEmpty && slotA.CanStack(slotB.item))
        {
            slotB.AddAmount(slotA.amount);
            slotA.Clear();
        }
        else
        {
            // Swap biasa
            (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
        }

        OnInventoryChanged?.Invoke();
    }

    // ─── Sort ─────────────────────────────────────────────────────────────────

    public void SortByName()
    {
        var filled = slots.Where(s => !s.IsEmpty).OrderBy(s => s.item.itemName).ToList();
        var empty = slots.Where(s => s.IsEmpty).ToList();
        slots = filled.Concat(empty).ToList();
        OnInventoryChanged?.Invoke();
    }

    public void SortByType()
    {
        var filled = slots.Where(s => !s.IsEmpty).OrderBy(s => s.item.itemType).ThenBy(s => s.item.itemName).ToList();
        var empty = slots.Where(s => s.IsEmpty).ToList();
        slots = filled.Concat(empty).ToList();
        OnInventoryChanged?.Invoke();
    }

    // ─── Search ───────────────────────────────────────────────────────────────

    public List<int> SearchIndices(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return slots.Select((_, i) => i).ToList();

        return slots
            .Select((slot, i) => (slot, i))
            .Where(x => !x.slot.IsEmpty &&
                x.slot.item.itemName.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0)
            .Select(x => x.i)
            .ToList();
    }

    // ─── Save / Load ──────────────────────────────────────────────────────────

    public List<InventoryItemSaveData> GetSaveSlots()
    {
        var result = new List<InventoryItemSaveData>();
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty)
            {
                result.Add(new InventoryItemSaveData
                {
                    slotIndex = i,
                    itemID = slots[i].item.itemID,
                    amount = slots[i].amount
                });
            }
        }
        return result;
    }

    public void LoadSlots(List<InventoryItemSaveData> savedSlots)
    {
        InitSlots();
        if (savedSlots == null) return;

        foreach (var saved in savedSlots)
        {
            if (saved.slotIndex >= slots.Count) continue;
            ItemData item = ItemDictionary.Instance.GetItemByID(saved.itemID);
            if (item == null) continue;
            slots[saved.slotIndex].Set(item, saved.amount);
        }

        OnInventoryChanged?.Invoke();
    }

    // Legacy support
    public List<ItemData> items => slots.Where(s => !s.IsEmpty).Select(s => s.item).ToList();
}