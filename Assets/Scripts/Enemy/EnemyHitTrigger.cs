using System.Collections.Generic;
using UnityEngine;

public class EnemyHitTrigger : MonoBehaviour
{
    public int damage = 5;

    HashSet<Collider2D> hitPlayer = new HashSet<Collider2D>();

    void OnEnable()
    {
        hitPlayer.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    void TryHit(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (hitPlayer.Contains(other)) return;

        hitPlayer.Add(other);

        Debug.Log("Hit: " + other.name);

        other.GetComponent<PlayerHealth>().TakeDamage(damage, this.transform.position);
        hitPlayer.Clear();
    }
}
