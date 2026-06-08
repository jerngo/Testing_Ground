using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> items = new List<ItemData>();
    
    void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        items.Add(item);
        Debug.Log("Item added: " + item.itemName);
    }

    public List<string> GetItemIDs()
    {
        List<string> ids = new List<string>();

        foreach (var item in items)
        {
            if (item != null)
                ids.Add(item.itemID);
        }

        return ids;
    }

    public void LoadItems(List<string> ids)
    {
        items.Clear();

        foreach (var id in ids)
        {
            ItemData item = ItemDictionary.Instance.GetItemByID(id);

            if (item != null)
            {
                items.Add(item);
            }
        }

        
    }
}
