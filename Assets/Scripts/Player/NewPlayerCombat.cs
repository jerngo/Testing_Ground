using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NewBasicPlatformerController2D controller;
    [SerializeField] Transform attackPoint;

    [Header("Attack")]
    [SerializeField] Vector2 attackSize = new Vector2(2f, 1f);
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int damage = 10;

    Vector3 attackPointStartLocalPos;

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<NewBasicPlatformerController2D>();

        if (attackPoint != null)
            attackPointStartLocalPos = attackPoint.localPosition;
    }

    void OnEnable()
    {
        if (controller != null)
            controller.OnAttackHit += DoAttackHit;
    }

    void OnDisable()
    {
        if (controller != null)
            controller.OnAttackHit -= DoAttackHit;
    }

    public void SetFacingDirection(float directionX)
    {
        if (attackPoint == null)
            return;

        if (Mathf.Approximately(directionX, 0))
            return;

        float sign = directionX > 0 ? 1 : -1;

        attackPoint.localPosition = new Vector3(
            Mathf.Abs(attackPointStartLocalPos.x) * sign,
            attackPointStartLocalPos.y,
            attackPointStartLocalPos.z
        );
    }

    void DoAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackPoint.position,
            attackSize,
            0,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<IDamageable>(out var damageable))
                continue;

            Debug.Log("Hit: " + hit.name);

            damageable.TakeDamage(damage, transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}