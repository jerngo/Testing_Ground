using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractPromptUI : MonoBehaviour
{
    public TMP_Text label;
    public Image icon;

    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // Selalu hadap kamera
        transform.rotation = Quaternion.LookRotation(
            transform.position - mainCam.transform.position
        );
    }

    public void Setup(string text, Sprite iconSprite = null)
    {
        label.text = text;

        if (icon != null)
        {
            icon.gameObject.SetActive(iconSprite != null);
            if (iconSprite != null) icon.sprite = iconSprite;
        }
    }
}