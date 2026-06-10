using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/Buff")]
public class BuffSkillEffect : SkillEffect
{
    public float speedMultiplier = 1.5f;
    public float duration = 5f;

    public override void Execute(GameObject caster)
    {
        caster.GetComponent<PlayerBuffHandler>()?.ApplySpeedBuff(speedMultiplier, duration);
    }
}