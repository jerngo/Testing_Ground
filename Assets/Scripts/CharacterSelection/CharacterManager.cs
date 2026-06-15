using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterManager : MonoBehaviour
{
    public event System.Action<GameObject> OnCharacterSpawned;

    public static CharacterManager Instance { get; private set; }

    [Header("Character Addressable Keys")]
    public string[] characterKeys = { "CharacterA", "CharacterB", "CharacterC" };

    [Header("Scene References (inject ke karakter baru)")]
    public GameSaveUI gameSaveUI;
    public InventoryUI inventoryUI;

    public PlayerController CurrentController { get; private set; }
    public PlayerInteraction CurrentInteraction { get; private set; }
    public SkillManager CurrentSkillManager { get; private set; }

    public GameObject currentCharacterObj;
    AsyncOperationHandle<GameObject> currentHandle;
    bool isSwapping = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(LoadDefaultThenApplySave());
    }

    /// <summary>Dipanggil saat player pilih karakter dari UI.</summary>
    public void SelectCharacter(int index)
    {
        if (isSwapping) return;
        if (index < 0 || index >= characterKeys.Length) return;
        StartCoroutine(SwapCharacter(characterKeys[index]));
    }

    IEnumerator SwapCharacter(string key, Vector3? overridePos = null)
    {
        isSwapping = true;

        Vector3 spawnPos = Vector3.zero;
        int carryHP = -1;

        if (currentCharacterObj != null)
        {
            // Ambil posisi dari child yang bergerak
            Transform playerChild = currentCharacterObj.transform.GetChild(0);
            spawnPos = playerChild.position;

            var oldHealth = currentCharacterObj.GetComponentInChildren<PlayerHealth>();
            if (oldHealth != null)
                carryHP = oldHealth.currentHP;

            if (currentHandle.IsValid())
                Addressables.ReleaseInstance(currentHandle);

            currentCharacterObj = null;
        }

        // Spawn di luar layar dulu, posisi diset manual setelah spawn
        var handle = Addressables.InstantiateAsync(key, new Vector3(0, -9999, 0), Quaternion.identity);
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[CharacterManager] Gagal load karakter: {key}");
            isSwapping = false;
            yield break;
        }

        currentHandle = handle;
        currentCharacterObj = handle.Result;

        // Set posisi manual + matikan physics dulu biar tidak jatuh
        // Set posisi ke child, bukan root
        Transform newPlayerChild = currentCharacterObj.transform.GetChild(0);
        var rb = newPlayerChild.GetComponent<Rigidbody2D>();

        if (rb != null) rb.simulated = false;
        newPlayerChild.position = overridePos ?? spawnPos;
        if (rb != null) rb.simulated = true;

        // Inject referensi
        CurrentController = currentCharacterObj.GetComponentInChildren<PlayerController>();
        CurrentInteraction = currentCharacterObj.GetComponentInChildren<PlayerInteraction>();
        CurrentSkillManager = currentCharacterObj.GetComponentInChildren<SkillManager>();

        // Auto-find kalau belum di-assign di Inspector
        if (gameSaveUI == null)
            gameSaveUI = FindFirstObjectByType<GameSaveUI>();
        if (inventoryUI == null)
            inventoryUI = FindFirstObjectByType<InventoryUI>();

        if (CurrentController != null)
        {
            CurrentController.gameSaveUI = gameSaveUI;
            CurrentController.inventoryUI = inventoryUI;
        }

        var newHealth = currentCharacterObj.GetComponentInChildren<PlayerHealth>();
        if (newHealth != null && carryHP >= 0)
            newHealth.currentHP = Mathf.Clamp(carryHP, 0, newHealth.maxHP);

        CharacterSelectionUI.Instance?.Hide();
        OnCharacterSpawned?.Invoke(currentCharacterObj);
        isSwapping = false;
    }

    /// <summary>Load karakter default saat game start (index 0).</summary>
    public void LoadDefault()
    {
        StartCoroutine(SwapCharacter(characterKeys[0]));
    }

    IEnumerator LoadDefaultThenApplySave()
    {
        Vector3 defaultPos = new Vector3(-25.28f, -4.12842989f, 0f);

        var saveData = SaveManager.Instance?.currentSaveData;
        bool hasSave = saveData != null && saveData.hp > 0 && saveData.hasData;

        // Pass posisi langsung ke SwapCharacter
        Vector3 startPos = hasSave ? new Vector3(saveData.posX, saveData.posY, 0f) : defaultPos;
        yield return StartCoroutine(SwapCharacter(characterKeys[0], startPos));

        if (hasSave)
            SaveManager.Instance.ApplyLoadedData();
    }
}