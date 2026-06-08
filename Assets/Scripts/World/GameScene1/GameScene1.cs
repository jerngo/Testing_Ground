using TMPro;
using UnityEngine;

public class GameScene1 : MonoBehaviour
{
    public TMP_InputField dataInput;
    void Start()
    {
        PlayerSaveData data = SaveManager.Instance.currentSaveData;
 
        if (data != null)
        {
            //dataInput.text = data.numberValue.ToString();
        }
        else
        {
            Debug.Log("No save data found");
        }
    }

    public void SaveData() {
        string username = AccountManager.Instance.currentAccount.username;
        int slot = SaveManager.Instance.currentSlot;

        PlayerSaveData data = new PlayerSaveData();
        int value;
        if (int.TryParse(dataInput.text, out value))
        {
            //data.numberValue = value;
        }
        else
        {
            Debug.LogError("Input bukan angka");
        }
        data.saveTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");

        SaveManager.Instance.Save(username, slot, data);
    }
}
