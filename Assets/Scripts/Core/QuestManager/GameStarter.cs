using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public QuestData startingQuest;

    void Awake()
    {
        QuestManager.Instance.StartQuest(startingQuest);
    }
}