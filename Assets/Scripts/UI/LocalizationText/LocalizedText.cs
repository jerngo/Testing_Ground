using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    public string key;

    TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    public void UpdateText()
    {
        textComponent.text = LocalizationManager.Instance.GetText(key);
    }
}
