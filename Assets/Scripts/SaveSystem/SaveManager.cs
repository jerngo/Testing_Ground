using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public QuestDatabase questDatabase;

    public PlayerSaveData currentSaveData;
    public int currentSlot;

    string saveFolder;

    const int MAX_SLOTS = 3;

    // AES key & IV — ganti dengan nilai unik untuk game kamu
    // Key harus 16, 24, atau 32 bytes; IV harus 16 bytes
    private static readonly byte[] AES_KEY = Encoding.UTF8.GetBytes("MySecretKey12345");   // 16 bytes
    private static readonly byte[] AES_IV = Encoding.UTF8.GetBytes("MySecretIV123456");   // 16 bytes

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
            return;
        }

        saveFolder = Application.persistentDataPath + "/saves/";

        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);
    }

    // ─── Path Helpers ─────────────────────────────────────────────────────────

    string GetAccountFolder(string username)
    {
        return saveFolder + username + "/";
    }

    string GetSlotPath(string username, int slot)
    {
        return GetAccountFolder(username) + "slot" + slot + ".sav";
    }

    // ─── Encryption / Decryption ──────────────────────────────────────────────

    private string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = AES_KEY;
            aes.IV = AES_IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();
                return System.Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    private string Decrypt(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = AES_KEY;
            aes.IV = AES_IV;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] cipherBytes = System.Convert.FromBase64String(cipherText);

            using (MemoryStream ms = new MemoryStream(cipherBytes))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
    }

    // ─── Core Save / Load ─────────────────────────────────────────────────────

    public bool Save(string username, int slot, PlayerSaveData data)
    {
        if (slot < 1 || slot > MAX_SLOTS)
        {
            Debug.LogError("Invalid Save Slot");
            return false;
        }

        try
        {
            long freeSpace = new DriveInfo(Path.GetPathRoot(Application.persistentDataPath)).AvailableFreeSpace;

            if (freeSpace < 1024 * 10)
            {
                Debug.LogError("Not enough disk space to save");
                NotificationUI.Instance.AddNotification("no_storage");
                return false;
            }

            string folder = GetAccountFolder(username);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.None);
            string encrypted = Encrypt(json);
            string path = GetSlotPath(username, slot);

            File.WriteAllText(path, encrypted, Encoding.UTF8);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save Failed: " + e.Message);
            NotificationUI.Instance.AddNotification("save_failed");
            return false;
        }
    }

    public PlayerSaveData Load(string username, int slot)
    {
        if (slot < 1 || slot > MAX_SLOTS)
        {
            Debug.LogError("Invalid Save Slot");
            return null;
        }

        string path = GetSlotPath(username, slot);

        if (!File.Exists(path))
            return new PlayerSaveData();

        try
        {
            string encrypted = File.ReadAllText(path, Encoding.UTF8);
            string json = Decrypt(encrypted);

            PlayerSaveData data = JsonConvert.DeserializeObject<PlayerSaveData>(json);
            currentSlot = slot;
            currentSaveData = data;
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Load Failed: " + e.Message);
            return new PlayerSaveData();
        }
    }

    // ─── Public API (sama seperti sebelumnya) ─────────────────────────────────

    public PlayerSaveData LoadPlayerData(int slot)
    {
        if (AccountManager.Instance.currentAccount == null)
        {
            Debug.Log("No account selected");
            return null;
        }

        string username = AccountManager.Instance.currentAccount.username;
        return Load(username, slot);
    }

    public bool SlotExists(string username, int slot)
    {
        return File.Exists(GetSlotPath(username, slot));
    }

    public void DeleteAccountSave(string username)
    {
        string folder = GetAccountFolder(username);

        try
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to delete account save: " + e.Message);
        }
    }

    public void DeleteSlot(string username, int slot)
    {
        string path = GetSlotPath(username, slot);

        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to delete save slot: " + e.Message);
        }
    }

    public void SaveNewGame(string username, int slot)
    {
        if (AccountManager.Instance.currentAccount == null)
        {
            Debug.LogError("No account selected");
            return;
        }

        PlayerSaveData data = new PlayerSaveData
        {
            hp = 100,
            posX = 0,
            posY = 0,
            username = username,
            saveTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm")
        };

        Save(username, slot, data);
    }

    public void SaveGame(string username, int slot)
    {
        if (AccountManager.Instance.currentAccount == null)
        {
            Debug.LogError("No account selected");
            return;
        }

        PlayerSaveData data = new PlayerSaveData();

        // Player
        var player = FindFirstObjectByType<PlayerHealth>();
        var playerTransform = player.transform;

        data.hp = player.currentHP;
        data.posX = playerTransform.localPosition.x;
        data.posY = playerTransform.localPosition.y;
        data.username = username;

        // Chest
        foreach (Chest chest in FindObjectsByType<Chest>(FindObjectsSortMode.None))
        {
            if (chest.IsOpened)
                data.openedChests.Add(chest.chestID);
        }

        // Enemy
        foreach (EnemyBase enemy in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            if (enemy.IsDead)
                data.defeatedEnemies.Add(enemy.enemyID);
        }

        // Inventory
        var inventory = InventoryManager.Instance;
        data.inventorySlots = inventory.GetSaveSlots();

        // Date
        data.saveTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");

        //Quest
        data.questProgress = QuestManager.Instance.GetSaveData();
        Save(username, slot, data);
    }

    public void ApplyLoadedData()
    {
        // Player
        var player = FindFirstObjectByType<PlayerHealth>();
        player.currentHP = currentSaveData.hp;
        player.transform.localPosition = new Vector2(currentSaveData.posX, currentSaveData.posY);

        // Chest
        foreach (Chest chest in FindObjectsByType<Chest>(FindObjectsSortMode.None))
        {
            if (currentSaveData.openedChests.Contains(chest.chestID))
                chest.SetOpened();
        }

        // Enemy
        foreach (EnemyBase enemy in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            if (currentSaveData.defeatedEnemies.Contains(enemy.enemyID))
                enemy.DieInstant();
        }

        // Inventory
        var inventory = InventoryManager.Instance;
        inventory.LoadSlots(currentSaveData.inventorySlots);

        // Quest
        Debug.Log("ApplyLoadedData: QuestManager.Instance = " + (QuestManager.Instance == null ? "NULL" : "OK"));
        QuestManager.Instance.LoadSaveData(
            currentSaveData.questProgress,
            questDatabase.allQuests
        );
    }
}