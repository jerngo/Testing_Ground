using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public string enemyID;

    public int maxHP = 100;
    public int currentHP;

    public bool isBlocking = false;

    Animator anim;

    bool isInvincible = false;
    public float invincibleTime = 0.5f;

    //PlayerController controller;

    Rigidbody2D rb;

    public float knockbackForce = 5f;
    public float knockbackUpForce = 2f;
    public bool isDead;

    BoarBehaviour behaviour;
    SpriteRenderer spriteRenderer;
    public void DieInstant()
    {
        Die();
    }

    void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();

        //controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        behaviour = GetComponent<BoarBehaviour>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, Vector2 hitSourcePosition)
    {
        if (isInvincible || currentHP <= 0) return;

        StartCoroutine(Invincible());

        

        int finalDamage = damage;

        if (isBlocking)
        {
            finalDamage = Mathf.RoundToInt(damage * 0.5f);
        }

        currentHP -= finalDamage;

        Debug.Log("Damage Taken: " + finalDamage + " | HP: " + currentHP);

        if (!isBlocking)
        {
            PlayHurt();
            ApplyKnockback(hitSourcePosition);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator Invincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    void PlayHurt()
    {
        if (anim != null)
        {
            anim.SetTrigger("Hurt");
        }
    }

    void Die()
    {
        // dead
        isDead = true;
        behaviour.enemyHit.gameObject.SetActive(false);
        behaviour.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void ApplyKnockback(Vector2 hitSourcePosition)
    {
        float direction = transform.position.x < hitSourcePosition.x ? -1f : 1f;

        Vector2 force = new Vector2(direction * knockbackForce, knockbackUpForce);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
