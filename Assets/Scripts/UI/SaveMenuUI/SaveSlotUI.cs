using TMPro;
using UnityEngine;

public class SaveSlotUI : MonoBehaviour
{
    public int slotIndex;

    public GameObject saveUI;
    public GameObject emptyUI;

    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI dateText;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (AccountManager.Instance.currentAccount == null)
            return;

        string username = AccountManager.Instance.currentAccount.username;

        bool hasSave = SaveManager.Instance.SlotExists(username, slotIndex);

        saveUI.SetActive(hasSave);
        emptyUI.SetActive(!hasSave);

        if (hasSave)
        {
            PlayerSaveData data = SaveManager.Instance.Load(username, slotIndex);

            usernameText.text = data.username;
            dateText.text = "Saved: " + data.saveTime;
        }
    }
}
