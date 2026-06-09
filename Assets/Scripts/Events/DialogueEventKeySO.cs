using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewDialogueEvent", menuName = "Dialogue/Event Key")]
public class DialogueEventKeySO : ScriptableObject
{
    private Action listeners;

    public void Register(Action callback) => listeners += callback;
    public void Unregister(Action callback) => listeners -= callback;
    public void Invoke() => listeners?.Invoke();
}