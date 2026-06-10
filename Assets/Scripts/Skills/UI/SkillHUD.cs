using UnityEngine;

public class SkillHUD : MonoBehaviour
{
    public SkillSlotUI[] slots;
    private static readonly string[] keys = { "Q", "R", "F" };

    void Start()
    {
        var sm = SkillManager.Instance;

        for (int i = 0; i < slots.Length; i++)
        {
            SkillData data = i < sm.skills.Length ? sm.skills[i] : null;
            slots[i].Init(i, data, keys[i]);
        }

        sm.OnCooldownUpdated += HandleCooldownUpdated;
    }

    void OnDestroy()
    {
        if (SkillManager.Instance != null)
            SkillManager.Instance.OnCooldownUpdated -= HandleCooldownUpdated;
    }

    void HandleCooldownUpdated(int index, float remaining, float total)
    {
        if (index < slots.Length)
            slots[index].SetCooldown(remaining, total);
    }
}