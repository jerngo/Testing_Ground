using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public PlayerSaveData currentSaveData;
    public int currentSlot;

    string saveFolder;

    const int MAX_SLOTS = 3;

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
        }

        saveFolder = Application.persistentDataPath + "/saves/";

        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);
    }

    string GetAccountFolder(string username)
    {
        return saveFolder + username + "/";
    }

    string GetSlotPath(string username, int slot)
    {
        return GetAccountFolder(username) + "slot" + slot + ".dat";
    }

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

            string path = GetSlotPath(username, slot);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();      
                formatter.Serialize(stream, data);
            }

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
        {
            return new PlayerSaveData();
        }

        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                PlayerSaveData data = (PlayerSaveData)formatter.Deserialize(stream);
                currentSlot = slot;
                currentSaveData = data;
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Load Failed: " + e.Message);
            return new PlayerSaveData();
        }
    }

    public bool SlotExists(string username, int slot)
    {
        string path = GetSlotPath(username, slot);
        return File.Exists(path);
    }

    public void DeleteAccountSave(string username)
    {
        string folder = GetAccountFolder(username);

        try
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to delete account slot: " + e.Message);
        }   
    }

    public void DeleteSlot(string username, int slot)
    {
        string path = GetSlotPath(username, slot);

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to delete save slot: " + e.Message);
        }
    }

    public void SaveNewGame(string username, int slot) {
        if (AccountManager.Instance.currentAccount == null)
        {
            Debug.LogError("No account selected");
            return;
        }

        PlayerSaveData data = new PlayerSaveData();

        // Player

        data.hp = 100;
        data.posX = 0;
        data.posY = 0;
        data.username = username;
        //Date
        data.saveTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");

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

        //Enemy
        foreach (EnemyBase enemy in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            if (enemy.IsDead)
            {
                data.defeatedEnemies.Add(enemy.enemyID);
            }
        }

        // Inventory
        var inventory = InventoryManager.Instance;
        data.inventoryItems = inventory.GetItemIDs();

        //Date
        data.saveTime = System.DateTime.Now.ToString("dd MMM yyyy HH:mm");

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
            {
                chest.SetOpened();
            }
        }

        // Enemy
        foreach (EnemyBase enemy in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            if (currentSaveData.defeatedEnemies.Contains(enemy.enemyID))
            {
                enemy.DieInstant();
            }
        }

        // Inventory
        var inventory = InventoryManager.Instance;
        inventory.LoadItems(currentSaveData.inventoryItems);
    }
}
