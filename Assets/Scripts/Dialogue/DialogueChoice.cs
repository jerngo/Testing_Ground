using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueChoice
{
    public string choiceText;
    public DialogueData nextDialogue; // null = akhiri dialogue

    [Space]
    public DialogueEventKeySO dialogueEvent;
}