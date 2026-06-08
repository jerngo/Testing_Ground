using UnityEngine;
using System.Collections;

public class BoarEnemy : EnemyBase
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;

    public float moveSpeed = 2f;
    public float waitTime = 1f;

    Transform targetPoint;

    [Header("Hitbox")]
    public Transform enemyHit;

    float hitOffsetX = 0.1170797f;

    protected override void Awake()
    {
        base.Awake();

        targetPoint = pointA;
    }

    void Start()
    {
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        while (!IsDead)
        {
            anim.SetBool("IsRunning", true);

            while (
                Vector2.Distance(
                    transform.position,
                    targetPoint.position
                ) > 0.1f
            )
            {
                Move();

                yield return null;
            }

            anim.SetBool("IsRunning", false);

            yield return new WaitForSeconds(waitTime);

            Flip();

            yield return new WaitForSeconds(waitTime);

            targetPoint =
                targetPoint == pointA
                ? pointB
                : pointA;
        }
    }

    void Move()
    {
        Vector2 direction =
            (targetPoint.position - transform.position)
            .normalized;

        transform.position +=
            (Vector3)direction *
            moveSpeed *
            Time.deltaTime;
    }

    void Flip()
    {
        sr.flipX = !sr.flipX;

        UpdateHitbox();
    }

    void UpdateHitbox()
    {
        Vector3 pos = enemyHit.localPosition;

        pos.x = sr.flipX
            ? hitOffsetX
            : -hitOffsetX;

        enemyHit.localPosition = pos;
    }

    // POLYMORPHISM
    protected override void Die()
    {
        base.Die();

        enemyHit.gameObject.SetActive(false);

        anim.SetTrigger("Death");

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);

        sr.enabled = false;
    }
}
