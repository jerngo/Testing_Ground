using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public PlayerHealth playerHealth;

    void Start()
    {
        if (playerHealth != null)
        {
            // Subscribe ke event
            playerHealth.OnHealthChanged += UpdateHealthUI;

            // Init awal
            UpdateHealthUI(playerHealth.currentHP, playerHealth.maxHP);
        }
    }

    void UpdateHealthUI(int currentHP, int maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            // Unsubscribe biar aman
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }
}
