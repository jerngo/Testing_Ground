using System;

public enum ObjectiveType
{
    KillEnemy,
    CollectItem
}

[Serializable]
public class QuestObjective
{
    public string objectiveID;
    public ObjectiveType type;
    public string targetID;         // enemyID atau itemID
    public string description;      // "Kill 3 Goblins" / "Collect 5 Wood"
    public int requiredAmount;
    public int currentAmount;

    public bool IsCompleted => currentAmount >= requiredAmount;

    public void AddProgress(int amount = 1)
    {
        currentAmount = Math.Min(currentAmount + amount, requiredAmount);
    }
}