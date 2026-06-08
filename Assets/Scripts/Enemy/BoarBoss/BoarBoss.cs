using System.Collections;
using UnityEngine;

public class BoarBoss : EnemyBase
{
    private PlayerController player;

    [Header("Detection & Attack Ranges")]
    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float attackRange = 2f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Charge Attack Settings")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 1f;
    [SerializeField] private float chargeCooldown = 3f;
    [SerializeField] private float chargeWindupTime = 0.5f;

    // State flags untuk kontrol logika yang lebih scalable
    private bool isCharging;
    private bool isCooldown;
    private bool isFacingRight = false;

    [Header("References")]
    [SerializeField] private Transform enemyHit;

    // Cache hash animasi untuk performa (menghindari string lookup setiap frame)
    private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        CurrentHP = maxHP;

        // Menggunakan scale untuk memudahkan flip seluruh hierarki objek (termasuk hitbox)
        transform.localScale = new Vector3(4f, 4f, 1f);

        InitPlayer();
    }

    private void InitPlayer()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }
    }

    void Update()
    {
        if (IsDead) return;

        // Fail-safe jika player belum ditemukan atau telah di-destroy/respawn
        if (player == null)
        {
            InitPlayer();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);

        HandleBehavior(distance);
    }

    private void HandleBehavior(float distance)
    {
        // 1. Prioritas Utama: Jika sedang menerjang, jangan lakukan logika lain
        if (isCharging) return;

        // 2. Logika Menyerang: Jika dekat dan tidak sedang cooldown
        if (distance <= attackRange && !isCooldown)
        {
            StartCoroutine(ChargeAttackRoutine());
            return;
        }

        // 3. Logika Mengejar: Jika masuk area deteksi
        if (distance <= detectRange)
        {
            ChasePlayer();
        }
        else
        {
            // Berhenti berlari jika player keluar dari jarak deteksi
            anim.SetBool(IsRunningHash, false);
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;

        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        Flip(direction.x);
        anim.SetBool(IsRunningHash, true);
    }

    IEnumerator ChargeAttackRoutine()
    {
        isCharging = true;
        isCooldown = true;
        anim.SetBool(IsRunningHash, false);

        // Animasi bersiap (Wind-up)
        anim.SetTrigger(AttackHash);
        yield return new WaitForSeconds(chargeWindupTime);

        // Kunci arah target sesaat sebelum menerjang (menghindari bos berputar di tengah jalan)
        Vector2 chargeDirection = (player.transform.position - transform.position).normalized;
        Flip(chargeDirection.x);

        float timer = 0f;
        while (timer < chargeDuration)
        {
            transform.position += (Vector3)chargeDirection * chargeSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // Selesai menerjang, bos bisa bergerak mengejar lagi selama masa cooldown
        isCharging = false;

        // Jeda sebelum bos bisa mengeluarkan serangan seruduk lagi
        yield return new WaitForSeconds(chargeCooldown);
        isCooldown = false;
    }

    private void Flip(float dirX)
    {
        // Menggunakan logika toleransi epsilon untuk menghindari flip konstan di titik 0
        if (Mathf.Abs(dirX) < 0.01f) return;

        bool shouldFaceRight = dirX > 0;

        if (isFacingRight != shouldFaceRight)
        {
            isFacingRight = shouldFaceRight;

            // Performa & Skalabilitas: Balik scale lokal X. 
            // Dengan cara ini, sprite DAN posisi lokal 'enemyHit' akan ikut berbalik otomatis secara akurat.
            Vector3 localScale = transform.localScale;
            localScale.x = shouldFaceRight ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
    }

    public override void TakeDamage(int damage, Vector2 hitPos)
    {
        // Pengganda kerusakan (0.7f) dipindah ke variabel atau dibiarkan seperti ini jika konstan
        int mitigatedDamage = Mathf.RoundToInt(damage * 0.7f);
        base.TakeDamage(mitigatedDamage, hitPos);
    }

    protected override void Die()
    {
        base.Die();

        if (enemyHit != null)
            enemyHit.gameObject.SetActive(false);

        anim.SetTrigger(DeathHash);
        StopAllCoroutines();
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);
        sr.enabled = false;
    }
}