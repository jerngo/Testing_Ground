using UnityEngine;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Info")]
    public string enemyID;

    [Header("Health")]
    [SerializeField] protected int maxHP = 100;

    protected int currentHP;

    public bool IsDead { get; protected set; }

    [Header("Defense")]
    [SerializeField] protected bool isBlocking;
    [SerializeField] protected float invincibleTime = 0.5f;

    [Header("Knockback")]
    [SerializeField] protected float knockbackForce = 5f;
    [SerializeField] protected float knockbackUpForce = 2f;

    protected bool isInvincible;

    protected Animator anim;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    // PROPERTY
    public int CurrentHP
    {
        get => currentHP;
        protected set
        {
            currentHP = Mathf.Clamp(value, 0, maxHP);
        }
    }

    // "Constructor" style
    protected virtual void Awake()
    {
        CurrentHP = maxHP;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public virtual void TakeDamage(int damage, Vector2 hitSourcePosition)
    {
        if (isInvincible || IsDead)
            return;

        StartCoroutine(Invincible());

        int finalDamage = isBlocking
            ? Mathf.RoundToInt(damage * 0.5f)
            : damage;

        CurrentHP -= finalDamage;

        Debug.Log($"{gameObject.name} HP: {CurrentHP}");

        if (!isBlocking)
        {
            PlayHurt();
            ApplyKnockback(hitSourcePosition);
        }

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator Invincible()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }

    protected virtual void PlayHurt()
    {
        anim?.SetTrigger("Hurt");
    }

    protected virtual void ApplyKnockback(Vector2 hitSourcePosition)
    {
        float direction =
            transform.position.x < hitSourcePosition.x
            ? -1f
            : 1f;

        Vector2 force = new Vector2(
            direction * knockbackForce,
            knockbackUpForce
        );

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public virtual void DieInstant()
    {
        Die();
    }

    protected virtual void Die()
    {
        IsDead = true;
        Debug.Log("ReportKill: " + enemyID);
        QuestManager.Instance.ReportKill(enemyID);
        Debug.Log(gameObject.name + " Dead");
    }
}
