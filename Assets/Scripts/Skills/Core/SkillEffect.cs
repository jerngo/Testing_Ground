using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    public abstract void Execute(GameObject caster);
}