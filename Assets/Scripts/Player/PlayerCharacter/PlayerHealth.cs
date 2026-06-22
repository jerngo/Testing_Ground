using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public Action<int, int> OnHealthChanged;

    public bool isBlocking = false;

    Animator anim;

    bool isInvincible = false;
    public float invincibleTime = 0.5f;

    PlayerController controller;
    NewBasicPlatformerController2D spineController;

    Rigidbody2D rb;

    public float knockbackForce = 5f;
    public float knockbackUpForce = 2f;

    void Start()
    {
        //currentHP = maxHP;
        anim = GetComponent<Animator>();

        controller = GetComponent<PlayerController>();
        spineController = GetComponent<NewBasicPlatformerController2D>();

        rb = GetComponent<Rigidbody2D>();

        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    public void TakeDamage(int damage, Vector2 hitSourcePosition)
    {
        if (isInvincible || currentHP <= 0) return;

        StartCoroutine(Invincible());

        bool isBlocking = controller != null && controller.isShielding;

        int finalDamage = damage;

        if (isBlocking)
        {
            finalDamage = Mathf.RoundToInt(damage * 0.5f);
        }

        currentHP -= finalDamage;

        OnHealthChanged?.Invoke(currentHP, maxHP);
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
        Debug.Log("Player Dead");

        if (anim != null)
            anim.SetTrigger("Death");

        if (controller != null)
            controller.IsDead = true;

        if (spineController != null)
            spineController.IsDead = true;

        GameManager.Instance.LoadSceneWithFade("MainMenuScene");
    }

    public void ApplyKnockback(Vector2 hitSourcePosition)
    {
        float direction = transform.position.x < hitSourcePosition.x ? -1f : 1f;

        Vector2 force = new Vector2(direction * knockbackForce, knockbackUpForce);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
