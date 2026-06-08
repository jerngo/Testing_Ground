using System.Collections;
using UnityEngine;

public class BoarBoss : EnemyBase
{
    PlayerController player;

    [Header("Detection")]
    public float detectRange = 8f;
    public float attackRange = 2f;

    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("Charge")]
    public float chargeSpeed = 10f;
    public float chargeDuration = 1f;
    public float chargeCooldown = 3f;

    bool isCharging;

    [Header("Hitbox")]
    public Transform enemyHit;

    float hitOffsetX = 0.1170797f;

    protected override void Awake()
    {
        base.Awake();

        //maxHP = 500;
        CurrentHP = maxHP;

        transform.localScale = Vector3.one * 4f;

        // Auto detect player
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (IsDead)
            return;

        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.transform.position
            );

        if (distance <= detectRange)
        {
            ChasePlayer();
        }

        if (
            distance <= attackRange &&
            !isCharging
        )
        {
            StartCoroutine(ChargeAttack());
        }
    }

    void ChasePlayer()
    {
        if (isCharging)
            return;

        Vector2 direction =
            (
                player.transform.position -
                transform.position
            ).normalized;

        transform.position +=
            (Vector3)direction *
            moveSpeed *
            Time.deltaTime;

        Flip(direction.x);

        anim.SetBool("IsRunning", true);
    }

    IEnumerator ChargeAttack()
    {
        isCharging = true;

        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        Vector2 direction =
            (
                player.transform.position -
                transform.position
            ).normalized;

        float timer = 0f;

        while (timer < chargeDuration)
        {
            transform.position +=
                (Vector3)direction *
                chargeSpeed *
                Time.deltaTime;

            timer += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(chargeCooldown);

        isCharging = false;
    }

    void Flip(float dirX)
    {
        if (dirX > 0)
            sr.flipX = true;
        else if (dirX < 0)
            sr.flipX = false;

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

    public override void TakeDamage(int damage, Vector2 hitPos)
    {
        damage = Mathf.RoundToInt(damage * 0.7f);

        base.TakeDamage(damage, hitPos);
    }

    protected override void Die()
    {
        base.Die();

        enemyHit.gameObject.SetActive(false);
        anim.SetTrigger("Death");

        StopAllCoroutines();

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);

        sr.enabled = false;
    }
}