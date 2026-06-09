using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public QuestData startingQuest;

    [Header("Dialogue Events")]
    public DialogueEventKeySO onQuestAccepted;
    public DialogueEventKeySO onQuestDeclined;

    void OnEnable()
    {
        onQuestAccepted?.Register(HandleQuestAccepted);
        onQuestDeclined?.Register(HandleQuestDeclined);
    }

    void OnDisable()
    {
        onQuestAccepted?.Unregister(HandleQuestAccepted);
        onQuestDeclined?.Unregister(HandleQuestDeclined);
    }

    void HandleQuestAccepted()
    {
        Debug.Log("HandleQuestAccepted dipanggil");
        Debug.Log($"QuestManager.Instance = {QuestManager.Instance}");
        bool result = QuestManager.Instance.StartQuest(startingQuest);
        Debug.Log($"StartQuest result = {result}");
    }

    void HandleQuestDeclined()
    {
        Debug.Log("Quest ditolak.");
    }
}