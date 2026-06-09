using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class QuestDialogueEntry
    {
        public QuestData quest;
        public DialogueData activeDialogue;   // saat quest ini sedang berjalan
        public DialogueData completedDialogue; // saat quest ini selesai
    }

    [Header("Dialogue")]
    public DialogueData defaultDialogue;

    [Header("Quest Dialogues")]
    public QuestDialogueEntry[] questDialogues; // urutan dari atas = prioritas tertinggi

    public void Interact(PlayerController controller)
    {
        if (DialogueManager.Instance.IsDialogueActive) return;

        DialogueData dialogueToPlay = GetDialogue();
        if (dialogueToPlay != null)
            DialogueManager.Instance.StartDialogue(dialogueToPlay);
    }

    DialogueData GetDialogue()
    {
        if (questDialogues == null || questDialogues.Length == 0)
            return defaultDialogue;

        // Cek dari atas ke bawah, ambil yang pertama match
        foreach (var entry in questDialogues)
        {
            if (entry.quest == null) continue;

            if (QuestManager.Instance.IsQuestActive(entry.quest.questID))
                return entry.activeDialogue != null ? entry.activeDialogue : defaultDialogue;

            if (QuestManager.Instance.IsQuestCompleted(entry.quest.questID))
                return entry.completedDialogue != null ? entry.completedDialogue : defaultDialogue;
        }

        return defaultDialogue;
    }
}