using System.Collections;
using UnityEngine;

// Kita tetap pertahankan RequireComponent agar memastikan komponen fisik selalu ada
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BoarBehaviour : EnemyBase
{
    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float arrivalTolerance = 0.1f;

    [Header("Combat/Hitbox")]
    [SerializeField] private Transform enemyHit;

    private Transform targetPoint;
    private Coroutine patrolCoroutine;
    private WaitForSeconds waitCache;
    private bool isFacingRight = true;

    // Cache Hash Animasi
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

    protected override void Awake()
    {
        // WAJIB: Memanggil Awake milik EnemyBase untuk inisialisasi HP, rb, anim, dan sr
        base.Awake();

        waitCache = new WaitForSeconds(waitTime);
    }

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"Poin patroli pada {gameObject.name} belum dipasang!");
            return;
        }

        targetPoint = pointA;
        patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        // Menggunakan properti 'IsDead' dari EnemyBase
        while (!IsDead)
        {
            anim.SetBool(IsRunningHash, true);

            while (HasNotReachedTarget())
            {
                // Jika sedang terkena knockback (knockback biasanya mematikan kontrol gerak sementara)
                // Kita bisa skip MoveToTarget agar tidak melawan gaya AddForce dari EnemyBase
                if (rb.linearVelocity.magnitude < 0.1f || isInvincible)
                {
                    MoveToTarget();
                }
                yield return new WaitForFixedUpdate();
            }

            anim.SetBool(IsRunningHash, false);
            rb.linearVelocity = Vector2.zero;

            yield return waitCache;

            SwitchTarget();
        }
    }

    bool HasNotReachedTarget()
    {
        float sqrDistance = (targetPoint.position - transform.position).sqrMagnitude;
        return sqrDistance > (arrivalTolerance * arrivalTolerance);
    }

    void MoveToTarget()
    {
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        UpdateFacing(direction.x);
    }

    void UpdateFacing(float moveX)
    {
        if (Mathf.Abs(moveX) < 0.01f) return;

        bool shouldFaceRight = moveX > 0;

        if (isFacingRight != shouldFaceRight)
        {
            isFacingRight = shouldFaceRight;

            Vector3 localScale = transform.localScale;
            localScale.x = shouldFaceRight ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
    }

    void SwitchTarget()
    {
        targetPoint = (targetPoint == pointA) ? pointB : pointA;
    }

    // OVERRIDE: Menyuntikkan logika tambahan saat Boar mati tanpa merusak base.Die()
    protected override void Die()
    {
        // Jalankan perintah dasar dari EnemyBase (IsDead = true, dll)
        base.Die();

        anim.SetBool(IsRunningHash, false);

        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
        }

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (enemyHit != null)
        {
            enemyHit.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, arrivalTolerance);
            Gizmos.DrawWireSphere(pointB.position, arrivalTolerance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}