using UnityEngine;

[CreateAssetMenu(fileName = "NewPassive", menuName = "Skills/Passive Data")]
public class PassiveData : ScriptableObject
{
    public string passiveName;
    public Sprite icon;
    public PassiveEffect effect;
}