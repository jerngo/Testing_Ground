using TMPro;
using UnityEngine;

public class AccountUI : MonoBehaviour
{
    public Transform content;
    public GameObject accountItemPrefab;
    public GameObject NoAccountText;
    void Start()
    {
        RefreshAccountList();
    }

    public void RefreshAccountList()
    {
        // hapus UI lama
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        var accounts = AccountManager.Instance.accounts;

        if (accounts.Count == 0)
        {
            NoAccountText.SetActive(true);
        }
        else {
            NoAccountText.SetActive(false);
        }

        foreach (var acc in accounts)
        {
            GameObject obj = Instantiate(accountItemPrefab, content);

            AccountItemUI ui = obj.GetComponent<AccountItemUI>();

            ui.Setup(acc.username, this);
        }
    }
}
