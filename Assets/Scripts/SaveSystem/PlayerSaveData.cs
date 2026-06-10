using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    // Player
    public int hp;
    public float posX;
    public float posY;
    public string username;

    // World state
    public List<string> openedChests = new List<string>();
    public List<string> defeatedEnemies = new List<string>();

    // Inventory
    //public List<string> inventoryItems = new List<string>();
    public List<InventoryItemSaveData> inventorySlots = new();

    // Quest
    public List<QuestProgress> questProgress = new List<QuestProgress>();

    // Meta
    public string saveTime;
}