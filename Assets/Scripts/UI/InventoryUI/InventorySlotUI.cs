using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("References")]
    public Image iconImage;
    public TMP_Text amountText;
    public GameObject highlight;

    public int SlotIndex { get; private set; }

    private InventorySlot data;

    public void Init(int index)
    {
        SlotIndex = index;
        Refresh();
    }

    public void Refresh()
    {
        if (SlotIndex >= InventoryManager.Instance.slots.Count) return;

        data = InventoryManager.Instance.slots[SlotIndex];

        bool hasItem = !data.IsEmpty;
        iconImage.gameObject.SetActive(hasItem);
        amountText.gameObject.SetActive(hasItem && data.amount > 1);

        if (hasItem)
        {
            iconImage.sprite = data.item.icon;
            amountText.text = data.amount.ToString();
        }
    }

    public void SetHighlight(bool active)
    {
        if (highlight != null)
            highlight.SetActive(active);
    }
}