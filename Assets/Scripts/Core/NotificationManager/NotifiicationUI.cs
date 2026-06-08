using TMPro;
using UnityEngine;

public class NotifiicationUI : MonoBehaviour
{
    public TMP_Text messageText;

    public void SetText(string msg)
    {
        messageText.text = msg;
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
