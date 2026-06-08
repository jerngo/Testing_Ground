using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestReward
{
    public int exp;
    public int gold;
    public List<ItemData> items = new List<ItemData>();  // drag ItemData asset di Inspector
}