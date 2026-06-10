using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("References")]
    public Image iconImage;
    public Image cooldownOverlay;
    public TMP_Text cooldownText;
    public TMP_Text keyLabel;

    public void Init(int index, SkillData data, string key)
    {
        keyLabel.text = key;

        if (data != null)
        {
            iconImage.sprite = data.icon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }

        SetCooldown(0, 1);
    }

    public void SetCooldown(float remaining, float total)
    {
        bool onCooldown = remaining > 0;
        cooldownOverlay.gameObject.SetActive(onCooldown);
        cooldownText.gameObject.SetActive(onCooldown);

        if (onCooldown)
        {
            cooldownOverlay.fillAmount = remaining / total;
            cooldownText.text = remaining.ToString("F1");
        }
    }
}