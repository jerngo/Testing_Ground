using UnityEngine;
using System.Collections.Generic;

public enum ObjectiveCompletionMode
{
    All,        // semua objective harus selesai
    Any,        // salah satu objective cukup
    Single      // hanya 1 objective
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class QuestData : ScriptableObject
{
    [Header("Identity")]
    public string questID;
    public string questName;
    [TextArea] public string description;

    [Header("Objectives")]
    public ObjectiveCompletionMode completionMode = ObjectiveCompletionMode.All;
    public List<QuestObjective> objectives = new List<QuestObjective>();

    [Header("Reward")]
    public bool hasReward;
    public QuestReward reward;

    [Header("Quest Chain")]
    public QuestData nextQuest;     // null jika tidak ada lanjutan
}