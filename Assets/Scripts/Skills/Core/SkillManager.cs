using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillManager : MonoBehaviour
{
    // Singleton tetap dipertahankan agar UI (Skill Slot) bisa mendeteksi cooldown dengan mudah via SkillManager.Instance
    public static SkillManager Instance { get; private set; }

    [Header("Skills (index 0=Q, 1=R, 2=F)")]
    public SkillData[] skills = new SkillData[3];

    private float[] cooldownTimers;
    private float[] totalCooldowns; // Cache untuk menyimpan durasi total cooldown asli

    public event System.Action<int, float, float> OnCooldownUpdated;

    void Awake()
    {
        // Karena menempel di Player, kita CUMA mengatur Instance. 
        // JANGAN gunakan DontDestroyOnLoad(gameObject) di sini agar tidak merusak hierarki objek Player saat pindah scene.
        Instance = this;

        cooldownTimers = new float[skills.Length];
        totalCooldowns = new float[skills.Length];
    }

    void Update()
    {
        // Validasi Game State jika ada
        if (GameStateManager.Instance != null && !GameStateManager.Instance.Is(GameState.Gameplay)) return;

        // Memproses cooldown setiap frame (In-place, hemat memori & CPU dibanding Coroutine)
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0)
            {
                cooldownTimers[i] -= Time.deltaTime;

                if (cooldownTimers[i] <= 0)
                {
                    cooldownTimers[i] = 0; // Mengunci angka di 0 bersih, anti-minus
                }

                OnCooldownUpdated?.Invoke(i, cooldownTimers[i], totalCooldowns[i]);
            }
        }
    }

    // ─── Input System Callbacks ───────────────────────────────────────────────
    // Pastikan PlayerInput component di GameObject Player memanggil fungsi-fungsi ini

    public void OnSkillQ(InputAction.CallbackContext ctx) { if (ctx.performed) UseSkill(0); }
    public void OnSkillR(InputAction.CallbackContext ctx) { if (ctx.performed) UseSkill(1); }
    public void OnSkillF(InputAction.CallbackContext ctx) { if (ctx.performed) UseSkill(2); }

    // ─── Public API ───────────────────────────────────────────────────────────

    public void UseSkill(int index)
    {
        if (GameStateManager.Instance != null && !GameStateManager.Instance.Is(GameState.Gameplay)) return;
        if (index >= skills.Length || skills[index] == null || skills[index].effect == null) return;
        if (cooldownTimers[index] > 0) return;

        // KARENA DIPASANG DI PLAYER: 
        // 'gameObject' secara otomatis merujuk ke GameObject Player itu sendiri.
        // Efek skill akan tahu siapa pemanggilnya dengan tepat.
        skills[index].effect.Execute(gameObject);

        // Set & catat cooldown awal
        totalCooldowns[index] = skills[index].cooldown;
        cooldownTimers[index] = totalCooldowns[index];

        // Trigger UI update pertama kali saat skill ditekan
        OnCooldownUpdated?.Invoke(index, cooldownTimers[index], totalCooldowns[index]);
    }

    public bool IsReady(int index) => index >= 0 && index < cooldownTimers.Length && cooldownTimers[index] <= 0;
    public float GetCooldownRemaining(int index) => index >= 0 && index < cooldownTimers.Length ? cooldownTimers[index] : 0;
    public float GetCooldownTotal(int index) => index >= 0 && index < totalCooldowns.Length ? totalCooldowns[index] : 0;
}