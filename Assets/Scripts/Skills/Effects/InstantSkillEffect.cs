using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/Instant")]
public class InstantSkillEffect : SkillEffect
{
    public int damage = 20;
    public float range = 3f;    // panjang ke depan
    public float width = 1.5f;  // lebar box
    public LayerMask targetLayer;
    public GameObject vfxPrefab;

    public override void Execute(GameObject caster)
    {
        SpriteRenderer sr = caster.GetComponent<SpriteRenderer>();
        bool facingLeft = sr != null && sr.flipX;
        Vector2 direction = facingLeft ? Vector2.left : Vector2.right;

        // Pusat box = posisi player + setengah range ke depan
        Vector2 boxCenter = (Vector2)caster.transform.position + direction * (range / 2f);
        Vector2 boxSize = new Vector2(range, width);

        if (vfxPrefab != null)
        {
            GameObject vfx = Object.Instantiate(vfxPrefab, boxCenter, Quaternion.identity);
            vfx.transform.localScale = new Vector3(range, width, 1f);
            Object.Destroy(vfx, 0.2f);
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, targetLayer);

        foreach (var hit in hits)
        {
            hit.GetComponent<IDamageable>()?.TakeDamage(damage, caster.transform.position);
        }
    }
}