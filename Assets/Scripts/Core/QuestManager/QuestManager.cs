using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    // Quest yang sedang berjalan: questID -> runtime copy QuestData
    private Dictionary<string, QuestData> activeQuests = new Dictionary<string, QuestData>();

    // Quest yang sudah selesai
    private HashSet<string> completedQuests = new HashSet<string>();

    // Event
    public event System.Action<QuestData> OnQuestStarted;
    public event System.Action<QuestData> OnQuestCompleted;
    public event System.Action<QuestData, QuestObjective> OnObjectiveUpdated;

    void Awake()
    {
        Debug.Log("QuestManager Awake, Instance = " + (Instance == null ? "NULL" : "OK"));
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("QuestManager Instance set");
        }
        else
        {
            Debug.Log("QuestManager duplicate, destroying");
            Destroy(gameObject);
        }
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    /// <summary>Mulai quest baru. Return false jika sudah aktif atau sudah selesai.</summary>
    public bool StartQuest(QuestData questData)
    {

        Debug.Log($"StartQuest: questData={questData}, id={questData?.questID}");

        if (questData == null) return false;
        if (activeQuests.ContainsKey(questData.questID)) return false;
        if (completedQuests.Contains(questData.questID)) return false;

        // Buat deep copy agar progress tidak mengubah ScriptableObject asli
        QuestData runtimeCopy = MakeRuntimeCopy(questData);
        activeQuests[runtimeCopy.questID] = runtimeCopy;

        Debug.Log($"[Quest] Started: {runtimeCopy.questName}");
        OnQuestStarted?.Invoke(runtimeCopy);

        return true;
    }

    /// <summary>Lapor kill enemy. Dipanggil dari EnemyBase saat mati.</summary>
    public void ReportKill(string enemyID)
    {
        ReportProgress(ObjectiveType.KillEnemy, enemyID, 1);
    }

    /// <summary>Lapor collect item. Dipanggil dari InventoryManager.</summary>
    public void ReportCollect(string itemID, int amount = 1)
    {
        ReportProgress(ObjectiveType.CollectItem, itemID, amount);
    }

    public bool IsQuestActive(string questID) => activeQuests.ContainsKey(questID);
    public bool IsQuestCompleted(string questID) => completedQuests.Contains(questID);

    public QuestData GetActiveQuest(string questID)
    {
        activeQuests.TryGetValue(questID, out QuestData q);
        return q;
    }

    public List<QuestData> GetAllActiveQuests() => activeQuests.Values.ToList();

    // ─── Progress ─────────────────────────────────────────────────────────────

    private void ReportProgress(ObjectiveType type, string targetID, int amount)
    {
        Debug.Log($"ReportProgress: type={type}, targetID={targetID}, activeQuests={activeQuests.Count}");
        // Iterasi semua active quest
        foreach (var quest in activeQuests.Values.ToList())
        {
            bool anyUpdated = false;

            foreach (var obj in quest.objectives)
            {
                if (obj.type != type) continue;
                if (obj.targetID != targetID) continue;
                if (obj.IsCompleted) continue;

                obj.AddProgress(amount);
                anyUpdated = true;
                OnObjectiveUpdated?.Invoke(quest, obj);
                Debug.Log($"[Quest] {quest.questName} | {obj.description}: {obj.currentAmount}/{obj.requiredAmount}");
            }

            if (anyUpdated)
                CheckQuestCompletion(quest);
        }
    }

    private void CheckQuestCompletion(QuestData quest)
    {
        bool completed = false;

        switch (quest.completionMode)
        {
            case ObjectiveCompletionMode.All:
                completed = quest.objectives.All(o => o.IsCompleted);
                break;
            case ObjectiveCompletionMode.Any:
                completed = quest.objectives.Any(o => o.IsCompleted);
                break;
            case ObjectiveCompletionMode.Single:
                completed = quest.objectives.Count > 0 && quest.objectives[0].IsCompleted;
                break;
        }

        if (completed)
            CompleteQuest(quest);
    }

    private void CompleteQuest(QuestData quest)
    {
        activeQuests.Remove(quest.questID);
        completedQuests.Add(quest.questID);

        Debug.Log($"[Quest] Completed: {quest.questName}");

        if (quest.hasReward && quest.reward != null)
            ApplyReward(quest.reward);

        OnQuestCompleted?.Invoke(quest);

        // Auto-start next quest
        if (quest.nextQuest != null)
        {
            StartQuest(quest.nextQuest);

            // Cek inventory untuk objective collect di quest baru
            foreach (var item in InventoryManager.Instance.items)
            {
                if (item != null)
                    ReportCollect(item.itemID, 1);
            }
        }
    }

    private void ApplyReward(QuestReward reward)
    {
        // EXP — sesuaikan dengan sistem exp kamu
        if (reward.exp > 0)
        {
            // PlayerStats.Instance.AddExp(reward.exp);
            Debug.Log($"[Quest] Reward: +{reward.exp} EXP");
        }

        // Gold — sesuaikan dengan sistem currency kamu
        if (reward.gold > 0)
        {
            // CurrencyManager.Instance.AddGold(reward.gold);
            Debug.Log($"[Quest] Reward: +{reward.gold} Gold");
        }

        // Items
        if (reward.items != null && reward.items.Count > 0)
        {
            var inventory = InventoryManager.Instance;
            foreach (ItemData item in reward.items)
            {
                if (item == null) continue;
                inventory.AddItem(item);
                Debug.Log($"[Quest] Reward item: {item.itemName}");
            }
        }
    }

    // ─── Save / Load ──────────────────────────────────────────────────────────

    /// <summary>Ambil data progress untuk disimpan ke PlayerSaveData.</summary>
    public List<QuestProgress> GetSaveData()
    {
        var result = new List<QuestProgress>();

        // Active quests — simpan progress objective
        foreach (var quest in activeQuests.Values)
        {
            var qp = new QuestProgress
            {
                questID = quest.questID,
                isCompleted = false
            };

            foreach (var obj in quest.objectives)
            {
                qp.objectiveProgress.Add(new QuestObjectiveProgress
                {
                    objectiveID = obj.objectiveID,
                    currentAmount = obj.currentAmount
                });
            }

            result.Add(qp);
        }

        // Completed quests
        foreach (var id in completedQuests)
        {
            result.Add(new QuestProgress { questID = id, isCompleted = true });
        }

        return result;
    }

    /// <summary>Restore quest state dari PlayerSaveData. Panggil setelah scene load.</summary>
    public void LoadSaveData(List<QuestProgress> savedQuests, List<QuestData> allQuestAssets)
    {
        // Jika tidak ada data tersimpan, jangan reset activeQuests
        if (savedQuests == null || savedQuests.Count == 0) return;

        activeQuests.Clear();
        completedQuests.Clear();

        activeQuests.Clear();
        completedQuests.Clear();

        if (savedQuests == null) return;

        // Buat lookup questID -> QuestData asset
        var assetLookup = allQuestAssets.ToDictionary(q => q.questID);

        foreach (var saved in savedQuests)
        {
            if (saved.isCompleted)
            {
                completedQuests.Add(saved.questID);
                continue;
            }

            if (!assetLookup.TryGetValue(saved.questID, out QuestData asset)) continue;

            QuestData runtimeCopy = MakeRuntimeCopy(asset);

            // Restore progress setiap objective
            var progressLookup = saved.objectiveProgress.ToDictionary(op => op.objectiveID);
            foreach (var obj in runtimeCopy.objectives)
            {
                if (progressLookup.TryGetValue(obj.objectiveID, out QuestObjectiveProgress op))
                    obj.currentAmount = op.currentAmount;
            }

            activeQuests[runtimeCopy.questID] = runtimeCopy;
        }

        Debug.Log($"[Quest] Loaded: {activeQuests.Count} active, {completedQuests.Count} completed");
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    /// Deep copy QuestData agar tidak modifikasi ScriptableObject asli
    private QuestData MakeRuntimeCopy(QuestData original)
    {
        QuestData copy = ScriptableObject.CreateInstance<QuestData>();
        copy.questID = original.questID;
        copy.questName = original.questName;
        copy.description = original.description;
        copy.completionMode = original.completionMode;
        copy.hasReward = original.hasReward;
        copy.reward = original.reward;
        copy.nextQuest = original.nextQuest;

        copy.objectives = new List<QuestObjective>();
        foreach (var obj in original.objectives)
        {
            copy.objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                type = obj.type,
                targetID = obj.targetID,
                description = obj.description,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }

        return copy;
    }
}