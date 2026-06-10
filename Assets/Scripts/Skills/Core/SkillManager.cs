using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Header("Skills (index 0=Q, 1=R, 2=F)")]
    public SkillData[] skills = new SkillData[3];

    private float[] cooldownTimers;
    private GameObject caster;

    public event System.Action<int, float, float> OnCooldownUpdated;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        cooldownTimers = new float[skills.Length];
        caster = gameObject;
    }

    public void OnSkillQ(InputAction.CallbackContext ctx) { if (ctx.performed) UseSkill(0); }
    public void OnSkillR(InputAction.CallbackContext ctx) { if (ctx.performed) UseSkill(1); }
    public void OnSkillF(InputAction.CallbackContext ctx) { if (ctx.performed) UseSkill(2); }

    public void UseSkill(int index)
    {
        if (!GameStateManager.Instance.Is(GameState.Gameplay)) return;
        if (index >= skills.Length || skills[index] == null) return;
        if (cooldownTimers[index] > 0) return;

        skills[index].effect.Execute(caster);
        StartCoroutine(RunCooldown(index));
    }

    IEnumerator RunCooldown(int index)
    {
        float total = skills[index].cooldown;
        cooldownTimers[index] = total;

        while (cooldownTimers[index] > 0)
        {
            cooldownTimers[index] -= Time.deltaTime;
            OnCooldownUpdated?.Invoke(index, cooldownTimers[index], total);
            yield return null;
        }

        cooldownTimers[index] = 0;
        OnCooldownUpdated?.Invoke(index, 0, total);
    }

    public bool IsReady(int index) => cooldownTimers[index] <= 0;
    public float GetCooldownRemaining(int index) => cooldownTimers[index];
    public float GetCooldownTotal(int index) => skills[index]?.cooldown ?? 0;
}