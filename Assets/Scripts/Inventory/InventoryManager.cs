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
        if (item == null || amount <= 0) return false;

        // 1. Jika stackable, cari slot pertama yang itemnya sama (karena tidak ada maxStack, langsung gabung semua)
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

        // 2. Jika tidak stackable ATAU tidak ada slot item sejenis, cari slot kosong
        var empty = slots.FirstOrDefault(s => s.IsEmpty);
        if (empty == null)
        {
            Debug.LogWarning("Inventory penuh!");
            return false;
        }

        empty.Set(item, amount);
        QuestManager.Instance?.ReportCollect(item.itemID, amount);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        // Cek total item di seluruh inventory dulu apakah cukup
        int totalAvailable = slots.Where(s => s.item == item).Sum(s => s.amount);
        if (totalAvailable < amount) return false;

        int amountToRemove = amount;

        // Iterasi dari belakang untuk mengurangi item (menangani jika ada duplikasi slot item sejenis)
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].item == item)
            {
                if (slots[i].amount > amountToRemove)
                {
                    slots[i].amount -= amountToRemove;
                    amountToRemove = 0;
                    break;
                }
                else
                {
                    amountToRemove -= slots[i].amount;
                    slots[i].Clear();
                }
            }
            if (amountToRemove <= 0) break;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA == indexB) return;
        if (indexA < 0 || indexB < 0 || indexA >= slots.Count || indexB >= slots.Count) return;

        var slotA = slots[indexA];
        var slotB = slots[indexB];

        // Jika sama item dan stackable — langsung merge total
        if (!slotA.IsEmpty && !slotB.IsEmpty && slotA.CanStack(slotB.item))
        {
            slotB.AddAmount(slotA.amount);
            slotA.Clear();
        }
        else
        {
            // PENTING: Tukar DATA-nya saja, jangan tukar referensi list (slots[indexA] = slots[indexB])
            // Supaya tidak merusak susunan indeks yang dibaca oleh UI Component kamu.
            ItemData tempItem = slotA.item;
            int tempAmount = slotA.amount;

            slotA.Set(slotB.item, slotB.amount);
            slotB.Set(tempItem, tempAmount);
        }

        OnInventoryChanged?.Invoke();
    }

    // ─── Sort (In-Place / Bebas GC Alloc) ───────────────────────────────────────

    public void SortByName()
    {
        // Menggunakan Array.Sort internal bawaan List (jauh lebih cepat & hemat memori dibanding LINQ)
        slots.Sort((a, b) =>
        {
            if (a.IsEmpty && b.IsEmpty) return 0;
            if (a.IsEmpty) return 1; // Slot kosong otomatis pindah ke belakang
            if (b.IsEmpty) return -1;
            return string.Compare(a.item.itemName, b.item.itemName, System.StringComparison.Ordinal);
        });

        OnInventoryChanged?.Invoke();
    }

    public void SortByType()
    {
        slots.Sort((a, b) =>
        {
            if (a.IsEmpty && b.IsEmpty) return 0;
            if (a.IsEmpty) return 1;
            if (b.IsEmpty) return -1;

            int typeCompare = string.Compare(a.item.itemType.ToString(), b.item.itemType.ToString(), System.StringComparison.Ordinal);
            if (typeCompare != 0) return typeCompare;

            return string.Compare(a.item.itemName, b.item.itemName, System.StringComparison.Ordinal);
        });

        OnInventoryChanged?.Invoke();
    }

    // ─── Search (Optimasi Loop) ────────────────────────────────────────────────

    public List<int> SearchIndices(string query)
    {
        var indices = new List<int>();
        bool isQueryEmpty = string.IsNullOrWhiteSpace(query);

        for (int i = 0; i < slots.Count; i++)
        {
            if (isQueryEmpty)
            {
                indices.Add(i);
                continue;
            }

            if (!slots[i].IsEmpty && slots[i].item.itemName.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                indices.Add(i);
            }
        }
        return indices;
    }

    //save load data
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
        InitSlots(); // Reset semua slot jadi kosong terlebih dahulu
        if (savedSlots == null) return;

        foreach (var saved in savedSlots)
        {
            if (saved.slotIndex >= slots.Count) continue;

            // Mengambil data item asli dari dictionary berdasarkan ID yang disimpan
            ItemData item = ItemDictionary.Instance.GetItemByID(saved.itemID);
            if (item == null) continue;

            slots[saved.slotIndex].Set(item, saved.amount);
        }

        OnInventoryChanged?.Invoke();
    }
}