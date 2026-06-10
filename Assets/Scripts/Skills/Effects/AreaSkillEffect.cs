using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/Area")]
public class AreaSkillEffect : SkillEffect
{
    public int damage = 50;
    public float radius = 4f;
    public LayerMask targetLayer;
    public GameObject vfxPrefab;

    public override void Execute(GameObject caster)
    {
        if (vfxPrefab != null)
        {
            float diameter = radius * 2f;
            GameObject vfx = Object.Instantiate(vfxPrefab, caster.transform.position, Quaternion.identity);
            vfx.transform.localScale = new Vector3(diameter, diameter, 1f);
            Object.Destroy(vfx, 0.2f);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.transform.position, radius, targetLayer
        );

        foreach (var hit in hits)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(damage, caster.transform.position);
        }
    }
}