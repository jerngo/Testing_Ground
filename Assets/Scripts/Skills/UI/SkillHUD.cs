using UnityEngine;

public class SkillHUD : MonoBehaviour
{
    public SkillSlotUI[] slots;
    private static readonly string[] keys = { "Q", "R", "F" };

    void OnEnable()
    {
        CharacterManager.Instance.OnCharacterSpawned += OnCharacterSpawned;
    }

    void OnDisable()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.OnCharacterSpawned -= OnCharacterSpawned;
    }

    void OnCharacterSpawned(GameObject character)
    {
        var sm = character.GetComponentInChildren<SkillManager>();
        if (sm == null) return;

        // Unsubscribe dari karakter lama dulu
        if (SkillManager.Instance != null)
            SkillManager.Instance.OnCooldownUpdated -= HandleCooldownUpdated;

        for (int i = 0; i < slots.Length; i++)
        {
            SkillData data = i < sm.skills.Length ? sm.skills[i] : null;
            slots[i].Init(i, data, keys[i]);
        }

        sm.OnCooldownUpdated += HandleCooldownUpdated;
    }

    void HandleCooldownUpdated(int index, float remaining, float total)
    {
        if (index < slots.Length)
            slots[index].SetCooldown(remaining, total);
    }
}
