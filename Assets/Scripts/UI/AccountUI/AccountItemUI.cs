using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountItemUI : MonoBehaviour
{
    public TextMeshProUGUI usernameText;

    string username;
    AccountUI listUI;

    public void Setup(string name, AccountUI parent)
    {
        username = name;
        listUI = parent;

        usernameText.text = name;
    }

    public void OnSelect()
    {
        var status = AccountManager.Instance.SwitchAccount(username);

        if (status == SaveStatus.Success)
        {
            Debug.Log("Selected account: " + username);

        
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public void OnDelete()
    {
        var status = AccountManager.Instance.DeleteAccount(username);

        if (status == SaveStatus.Success)
        {
            listUI.RefreshAccountList();
        }
    }
}
