using System.Collections.Generic;
using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    public static PassiveManager Instance { get; private set; }

    [Header("Passives")]
    public List<PassiveData> passives = new();

    private GameObject owner;
    private PlayerHealth playerHealth;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        owner = gameObject;
    }

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();

        // Aktifkan semua passive Always
        foreach (var passive in passives)
        {
            if (passive?.effect?.trigger == PassiveTrigger.Always)
                passive.effect.OnActivate(owner);
        }
    }

    void OnDestroy()
    {
        // Deaktifkan semua passive Always
        foreach (var passive in passives)
        {
            if (passive?.effect?.trigger == PassiveTrigger.Always)
                passive.effect.OnDeactivate(owner);
        }
    }

    // ─── Trigger Methods ──────────────────────────────────────────────────────

    public void TriggerOnKill()
    {
        foreach (var passive in passives)
        {
            if (passive?.effect?.trigger == PassiveTrigger.OnKillEnemy)
                passive.effect.OnActivate(owner);
        }
    }

    public void TriggerOnTakeDamage()
    {
        foreach (var passive in passives)
        {
            if (passive?.effect?.trigger == PassiveTrigger.OnTakeDamage)
                passive.effect.OnActivate(owner);
        }
    }

    //public void CheckLowHP(int currentHP, int maxHP)
    //{
    //    float ratio = (float)currentHP / maxHP;

    //    foreach (var passive in passives)
    //    {
    //        if (passive?.effect is LowHPPassive lowHP)
    //        {
    //            if (ratio <= lowHP.hpThreshold)
    //                lowHP.OnActivate(owner);
    //            else
    //                lowHP.OnDeactivate(owner);
    //        }
    //    }
    //}
}