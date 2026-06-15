using UnityEngine;

public class GameSaveUI : MonoBehaviour
{
    public GameObject savePanel;
    public SaveSlotHolder saveSlotHolder;
    bool isOpen = false;

    void Awake()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.gameSaveUI = this;
    }

    void Start()
    {
        savePanel.SetActive(false);
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        savePanel.SetActive(isOpen);

        if (isOpen)
        {
            saveSlotHolder.RefreshAll();
        }
    }
}
