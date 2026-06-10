using UnityEngine;
using UnityEngine.Serialization;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Quest,
    Misc
}

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemID;
    public string itemName;
    [FormerlySerializedAs("itemIcon")]
    public Sprite icon;        
    [FormerlySerializedAs("type")]
    public ItemType itemType;  

    [Header("Stack")]
    public bool isStackable;

    [Header("Stats")]
    public int attack;
    public int defense;
}