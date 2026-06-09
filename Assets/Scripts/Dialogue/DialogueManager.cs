using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Reference")]
    public DialogueUI dialogueUI;

    private DialogueData currentData;
    private int currentLineIndex;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.lines.Length == 0) return;
        currentData = data;
        currentLineIndex = 0;
        dialogueUI.Show();
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        DialogueLine line = currentData.lines[currentLineIndex];

        line.dialogueEvent?.Invoke();

        bool hasChoices = line.choices != null && line.choices.Length > 0;
        System.Action<int> choiceCallback = hasChoices ? OnChoiceSelected : null;
        dialogueUI.DisplayLine(line, choiceCallback, NextLine);
    }

    public void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex >= currentData.lines.Length)
        {
            EndDialogue();
            return;
        }
        ShowCurrentLine();
    }

    void OnChoiceSelected(int choiceIndex)
    {
        DialogueLine currentLine = currentData.lines[currentLineIndex];
        DialogueChoice chosen = currentLine.choices[choiceIndex];

        chosen.dialogueEvent?.Invoke();

        if (chosen.nextDialogue == null)
        {
            NextLine();
            return;
        }

        StartDialogue(chosen.nextDialogue);
    }

    void EndDialogue()
    {
        currentData = null;
        dialogueUI.Hide();
    }

    public bool IsDialogueActive => currentData != null;
}