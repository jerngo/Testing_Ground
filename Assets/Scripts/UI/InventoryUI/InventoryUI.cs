using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public GameObject inventoryPanel;
    public Transform gridParent;
    public GameObject slotPrefab;
    public Canvas canvas;

    [Header("Controls")]
    public TMP_InputField searchField;
    public Button sortNameButton;
    public Button sortTypeButton;

    private List<InventorySlotUI> slotUIs = new();
    private List<int> highlightedIndices = new();

    void Awake()
    {
        if (CharacterManager.Instance != null)
            CharacterManager.Instance.inventoryUI = this;
    }

    void Start()
    {
        GenerateGrid();

        InventoryManager.Instance.OnInventoryChanged += RefreshAll;

        if (searchField != null)
            searchField.onValueChanged.AddListener(OnSearch);

        if (sortNameButton != null)
            sortNameButton.onClick.AddListener(() =>
            {
                InventoryManager.Instance.SortByName();
                if (searchField != null) searchField.text = "";
            });

        if (sortTypeButton != null)
            sortTypeButton.onClick.AddListener(() =>
            {
                InventoryManager.Instance.SortByType();
                if (searchField != null) searchField.text = "";
            });

        inventoryPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshAll;
    }

    void GenerateGrid()
    {
        int count = InventoryManager.Instance.slotCount;
        Debug.Log($"count={count}, slotPrefab={slotPrefab}, gridParent={gridParent}");

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(slotPrefab, gridParent);
            Debug.Log($"slot {i} instantiated={obj}");

            var slotUI = obj.GetComponent<InventorySlotUI>();
            Debug.Log($"slotUI={slotUI}");

            var dragHandler = obj.GetComponent<ItemDragHandler>();
            Debug.Log($"dragHandler={dragHandler}");

            slotUI.Init(i);
            dragHandler.slotUI = slotUI;

            slotUIs.Add(slotUI);
        }
    }

    void RefreshAll()
    {
        foreach (var slot in slotUIs)
        {
            if (slot != null)
                slot.Refresh();
        }

        if (searchField != null && !string.IsNullOrWhiteSpace(searchField.text))
            OnSearch(searchField.text);
    }

    void OnSearch(string query)
    {
        // Reset semua highlight
        foreach (var slot in slotUIs)
            slot.SetHighlight(false);

        if (string.IsNullOrWhiteSpace(query)) return;

        var matchIndices = InventoryManager.Instance.SearchIndices(query);
        foreach (int i in matchIndices)
        {
            if (i < slotUIs.Count)
                slotUIs[i].SetHighlight(true);
        }
    }

    public void ToggleInventory()
    {
        bool active = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(active);
        GameStateManager.Instance.SetState(active ? GameState.Menu : GameState.Gameplay);
    }
}