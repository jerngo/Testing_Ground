using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(2, 5)]
    public string text;
    public DialogueChoice[] choices;

    [Space]
    public DialogueEventKeySO dialogueEvent;
}