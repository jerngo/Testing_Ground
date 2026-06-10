using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Info")]
    public string skillName;
    public Sprite icon;

    [Header("Cooldown")]
    public float cooldown = 5f;

    [Header("Effect")]
    public SkillEffect effect;
}