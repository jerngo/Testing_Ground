using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public int damage = 40;

    HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    void OnEnable()
    {
        hitEnemies.Clear();
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
        if (hitEnemies.Contains(other))
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        hitEnemies.Add(other);

        Debug.Log("Hit: " + other.name);

        damageable.TakeDamage(damage, transform.position);
    }
}
