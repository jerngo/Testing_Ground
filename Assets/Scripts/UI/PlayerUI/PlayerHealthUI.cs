using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    private PlayerHealth currentPlayerHealth;

    void Start()
    {
        CharacterManager.Instance.OnCharacterSpawned += OnCharacterSpawned;
    }

    void OnDestroy()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.OnCharacterSpawned -= OnCharacterSpawned;

        // Unsubscribe dari karakter lama
        if (currentPlayerHealth != null)
            currentPlayerHealth.OnHealthChanged -= UpdateHealthUI;
    }

    void OnCharacterSpawned(GameObject character)
    {
        // Unsubscribe dari karakter lama dulu
        if (currentPlayerHealth != null)
            currentPlayerHealth.OnHealthChanged -= UpdateHealthUI;

        currentPlayerHealth = character.GetComponentInChildren<PlayerHealth>();

        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(currentPlayerHealth.currentHP, currentPlayerHealth.maxHP);
        }
    }

    void UpdateHealthUI(int currentHP, int maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
    }
}