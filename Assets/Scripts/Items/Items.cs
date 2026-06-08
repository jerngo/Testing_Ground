using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public Sprite itemIcon;
    public string itemName;
    public ItemType type;

    public int attack;
    public int defense;
}
