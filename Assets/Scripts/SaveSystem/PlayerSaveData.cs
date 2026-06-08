using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public string username;
    public int hp;
    public float posX;
    public float posY;

    public List<string> openedChests = new List<string>();
    public List<string> defeatedEnemies = new List<string>();

    public List<string> inventoryItems = new List<string>();
    public string saveTime;
}