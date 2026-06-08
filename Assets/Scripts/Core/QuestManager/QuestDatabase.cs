using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Taruh semua QuestData asset di sini.
/// Digunakan oleh QuestManager.LoadSaveData() untuk resolve questID -> QuestData.
/// </summary>
[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest System/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestData> allQuests = new List<QuestData>();
}