using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TMPro;
using UnityEngine;

[System.Serializable]
public class AccountManager : MonoBehaviour
{
    public const int MAX_ACCOUNTS = 4;

    public static AccountManager Instance;

    public List<UserAccount> accounts = new List<UserAccount>();

    public UserAccount currentAccount;

    string accountFile;

    public TextMeshProUGUI selectedUsernameText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            accountFile = Path.Combine(Application.persistentDataPath, "accounts.dat");

            LoadAccounts();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    SaveStatus LoadAccounts()
    {
        if (!File.Exists(accountFile))
            return SaveStatus.Success;

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(accountFile, FileMode.Open);

            AccountList data = formatter.Deserialize(stream) as AccountList;

            accounts = data.accounts;
            
            stream.Close();

            return SaveStatus.Success;
        }
        catch
        {
            accounts = new List<UserAccount>();

            return SaveStatus.FileCorrupted;
        }
    }

    SaveStatus SaveAccounts()
    {
        try
        {
            if (!HasEnoughDiskSpace(1024 * 10)) {
                NotificationUI.Instance.AddNotification("no_storage");
                return SaveStatus.NoFreeSpace;
            
            }

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(accountFile, FileMode.Create);

            AccountList data = new AccountList();
            data.accounts = accounts;

            formatter.Serialize(stream, data);

            stream.Close();

            return SaveStatus.Success;
        }
        catch
        {
            return SaveStatus.WriteFailed;
        }
    }

    public SaveStatus CreateAccount(string username)
    {
        username = SanitizeUsername(username);

        if (string.IsNullOrEmpty(username))
            return SaveStatus.InvalidUsername;

        if (accounts.Count >= MAX_ACCOUNTS)
            return SaveStatus.AccountLimitReached;

        if (accounts.Exists(a => a.username == username))
            return SaveStatus.AccountAlreadyExist;

        UserAccount newAccount = new UserAccount();
        newAccount.username = username;

        accounts.Add(newAccount);

        CreateAccountFolder(username);

        return SaveAccounts();
    }

    void CreateAccountFolder(string username)
    {
        string path = Path.Combine(Application.persistentDataPath, "SaveData", username);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public SaveStatus DeleteAccount(string username)
    {
        accounts.RemoveAll(a => a.username == username);
        SaveManager.Instance.DeleteAccountSave(username);

        if (currentAccount != null && currentAccount.username == username)
        {
            currentAccount = null;
        }

        string path = Path.Combine(
            Application.persistentDataPath,
            "SaveData",
            username
        );

        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            return SaveAccounts();
        }
        catch
        {
            return SaveStatus.WriteFailed;
        }
    }

    public SaveStatus SwitchAccount(string username)
    {
        UserAccount account = accounts.Find(a => a.username == username);

        if (account == null) {
            selectedUsernameText.text = "";
            return SaveStatus.AccountNotFound;
        }

        currentAccount = account;
        selectedUsernameText.text = username;
        Debug.Log("Switched to account: " + username);

        return SaveStatus.Success;
    }

    public string GetCurrentSavePath()
    {
        string path = Path.Combine(
            Application.persistentDataPath,
            "SaveData",
            currentAccount.username,
            "save.json"
        );

        return path;
    }

    bool HasEnoughDiskSpace(long requiredBytes)
    {
        try
        {
            string root = Path.GetPathRoot(Application.persistentDataPath);
            DriveInfo drive = new DriveInfo(root);

            return drive.AvailableFreeSpace > requiredBytes;
        }
        catch
        {
            return true;
        }
    }

    string SanitizeUsername(string username)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();

        StringBuilder builder = new StringBuilder();

        foreach (char c in username)
        {
            if (!System.Array.Exists(invalidChars, x => x == c))
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Trim();
    }
}
