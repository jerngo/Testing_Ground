using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    Dictionary<string, string> localizedText = new Dictionary<string, string>();

    public string currentLanguage = "en";

    public System.Action OnLanguageChanged;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            string savedLang = PlayerPrefs.GetString("LANGUAGE", "en");
            LoadLanguage(savedLang);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void LoadLanguage(string language)
    {
        currentLanguage = language;

        PlayerPrefs.SetString("LANGUAGE", language);
        PlayerPrefs.Save();

        TextAsset jsonFile = Resources.Load<TextAsset>("Localization/" + language);

        if (jsonFile == null)
        {
            Debug.LogError("Language file not found: " + language);
            return;
        }

        LocalizationData data = JsonUtility.FromJson<LocalizationData>(jsonFile.text);

        localizedText.Clear();

        foreach (var item in data.items)
        {
            localizedText[item.key] = item.value;
        }

        OnLanguageChanged?.Invoke(); 
    }

    public string GetText(string key)
    {
        if (localizedText.ContainsKey(key))
            return localizedText[key];

        return key;
    }
}
