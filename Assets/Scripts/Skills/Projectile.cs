using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 15;
    public LayerMask targetLayer;
    public float lifetime = 3f;

    private Vector2 direction;
    private float speed;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 dir, float spd)
    {
        direction = dir;
        speed = spd;
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0) return;
        other.GetComponent<IDamageable>()?.TakeDamage(damage, transform.position);
        Destroy(gameObject);
    }
}