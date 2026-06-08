using TMPro;
using UnityEngine;

public class CreateAccountUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public AccountUI accountListUI;
    public GameObject CreateAccountUICanvas;
    public void OnCreateAccount()
    {
        string username = usernameInput.text;

        var status = AccountManager.Instance.CreateAccount(username);

        if (status == SaveStatus.Success)
        {
            usernameInput.text = "";
            accountListUI.RefreshAccountList();
            CreateAccountUICanvas.SetActive(false);
        }
        else
        {
            Debug.Log("Create account failed: " + status);
        }
    }
}
