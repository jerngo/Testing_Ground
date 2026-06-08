using UnityEngine;
using TMPro;
using System.Collections;

public class QuestHUD : MonoBehaviour
{
    [Header("References")]
    public GameObject hudPanel;
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI objectivesText;

    void OnEnable()
    {
        QuestManager.Instance.OnQuestStarted += HandleQuestStarted;
        QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;
        QuestManager.Instance.OnObjectiveUpdated += HandleObjectiveUpdated;
    }

    void OnDisable()
    {
        QuestManager.Instance.OnQuestStarted -= HandleQuestStarted;
        QuestManager.Instance.OnQuestCompleted -= HandleQuestCompleted;
        QuestManager.Instance.OnObjectiveUpdated -= HandleObjectiveUpdated;
    }

    void Start()
    {
        // Cek apakah sudah ada quest aktif saat scene load (misal dari save)
        var activeQuests = QuestManager.Instance.GetAllActiveQuests();
        if (activeQuests.Count > 0)
            RefreshHUD(activeQuests[0]);
        else
            hudPanel.SetActive(false);
    }

    void HandleQuestStarted(QuestData quest)
    {
        RefreshHUD(quest);
    }

    void HandleQuestCompleted(QuestData quest)
    {
        StartCoroutine(RefreshAfterChain());
    }

    IEnumerator RefreshAfterChain()
    {
        yield return null; // tunggu 1 frame, next quest sudah masuk activeQuests
        var activeQuests = QuestManager.Instance.GetAllActiveQuests();
        if (activeQuests.Count > 0)
            RefreshHUD(activeQuests[0]);
        else
            hudPanel.SetActive(false);
    }

    void HandleObjectiveUpdated(QuestData quest, QuestObjective objective)
    {
        RefreshHUD(quest);
    }

    void RefreshHUD(QuestData quest)
    {
        hudPanel.SetActive(true);
        questNameText.text = quest.questName;

        // Tulis semua objective
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var obj in quest.objectives)
        {
            string status = obj.IsCompleted ? "<s>" : "";
            string statusEnd = obj.IsCompleted ? "</s>" : "";
            sb.AppendLine($"{status}{obj.description}  {obj.currentAmount}/{obj.requiredAmount}{statusEnd}");
        }

        objectivesText.text = sb.ToString().TrimEnd();
    }
}