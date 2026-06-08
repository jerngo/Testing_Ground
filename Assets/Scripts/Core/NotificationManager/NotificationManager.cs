using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public GameObject notifPrefab;
    public Transform notifParent;

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
    }

    public void ShowNotification(string key)
    {
        string localizedText =
       LocalizationManager.Instance.GetText(key);

       NotificationUI.Instance.AddNotification(key);
    }
}
