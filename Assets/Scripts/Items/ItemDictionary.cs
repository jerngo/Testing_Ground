using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public static ItemDictionary Instance;

    public List<ItemData> allItems;

    Dictionary<string, ItemData> itemDict;

    void Awake()
    {
        Instance = this;

        itemDict = new Dictionary<string, ItemData>();

        foreach (var item in allItems)
        {
            itemDict[item.itemID] = item;
        }
    }

    public ItemData GetItemByID(string id)
    {
        if (itemDict.ContainsKey(id))
            return itemDict[id];

        Debug.LogWarning("Item ID not found: " + id);
        return null;
    }
}
