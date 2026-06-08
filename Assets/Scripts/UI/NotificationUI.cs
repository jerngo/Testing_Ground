using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI Instance;

    public TextMeshProUGUI notificationText;
    public GameObject panel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        panel.SetActive(false);
    }

    public void AddNotification(string key)
    {
        string localizedText = LocalizationManager.Instance.GetText(key);
        panel.SetActive(true);
        notificationText.text = localizedText;
    }

}
