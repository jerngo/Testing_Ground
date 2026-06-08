using System;
using System.Collections.Generic;

[Serializable]
public class QuestObjectiveProgress
{
    public string objectiveID;
    public int currentAmount;
}

[Serializable]
public class QuestProgress
{
    public string questID;
    public bool isCompleted;
    public List<QuestObjectiveProgress> objectiveProgress = new List<QuestObjectiveProgress>();
}