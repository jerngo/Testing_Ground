using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotHolder : MonoBehaviour
{
    public SaveSlotUI[] slots;

    void Start()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        foreach (var slot in slots)
        {
            slot.Refresh();
        }
    }

    public void OnClickLoadSlot(int slot)
    {
        string username = AccountManager.Instance.currentAccount.username;

        PlayerSaveData data = SaveManager.Instance.Load(username, slot);
        SceneManager.LoadScene("GameLevel");
        
    }

    public void OnClickCreateSave(int slot)
    {
        string username = AccountManager.Instance.currentAccount.username;
        

        SaveManager.Instance.SaveNewGame(username, slot);
        PlayerSaveData data = SaveManager.Instance.Load(username, slot);
        SceneManager.LoadScene("GameLevel");
    }

    public void OnClickReplaceSave(int slot)
    {
        string username = AccountManager.Instance.currentAccount.username;

        SaveManager.Instance.SaveGame(username, slot);
        NotificationManager.Instance.ShowNotification("save_success");
        RefreshAll();

    }

    public void OnDeleteSlot(int slotIndex)
    {
        if (AccountManager.Instance.currentAccount == null)
            return;

        string username = AccountManager.Instance.currentAccount.username;

        SaveManager.Instance.DeleteSlot(username, slotIndex);

        RefreshAll();
    }

}
