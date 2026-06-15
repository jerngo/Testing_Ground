using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Passive/StatBoost")]
public class StatBoostPassive : PassiveEffect
{
    public float walkSpeedBonus = 1f;
    public float runSpeedBonus = 2f;

    public override void OnActivate(GameObject owner)
    {
        var controller = owner.GetComponent<PlayerController>();
        if (controller == null) return;

        controller.walkSpeed += walkSpeedBonus;
        controller.runSpeed += runSpeedBonus;
    }

    public override void OnDeactivate(GameObject owner)
    {
        // Guard: owner sudah destroyed
        if (owner == null) return;

        var controller = owner.GetComponent<PlayerController>();
        if (controller == null) return;

        controller.walkSpeed -= walkSpeedBonus;
        controller.runSpeed -= runSpeedBonus;
    }
}