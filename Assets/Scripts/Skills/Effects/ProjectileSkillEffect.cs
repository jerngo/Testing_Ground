using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Effects/Projectile")]
public class ProjectileSkillEffect : SkillEffect
{
    public GameObject projectilePrefab;
    public float speed = 10f;

    public override void Execute(GameObject caster)
    {
        SpriteRenderer sr = caster.GetComponent<SpriteRenderer>();
        Vector2 direction = sr != null && sr.flipX ? Vector2.left : Vector2.right;

        GameObject obj = Object.Instantiate(
            projectilePrefab,
            caster.transform.position,
            Quaternion.identity
        );

        obj.GetComponent<Projectile>()?.Launch(direction, speed);
    }
}