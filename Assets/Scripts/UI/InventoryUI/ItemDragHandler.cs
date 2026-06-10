using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("References")]
    public InventorySlotUI slotUI;
    public Canvas canvas;

    private GameObject dragGhost;
    private CanvasGroup ghostCanvasGroup;
    private RectTransform ghostRect;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>(true);
        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = InventoryManager.Instance.slots[slotUI.SlotIndex];
        if (slot.IsEmpty) { eventData.pointerDrag = null; return; }

        dragGhost = new GameObject("DragGhost");
        dragGhost.transform.SetParent(canvas.transform, false);
        dragGhost.transform.SetAsLastSibling();

        var img = dragGhost.AddComponent<Image>();
        img.sprite = slot.item.icon;
        img.raycastTarget = false;

        ghostCanvasGroup = dragGhost.AddComponent<CanvasGroup>();
        ghostCanvasGroup.alpha = 0.7f;
        ghostCanvasGroup.blocksRaycasts = false;

        ghostRect = dragGhost.GetComponent<RectTransform>();
        ghostRect.sizeDelta = new Vector2(60, 60);

        // Set posisi awal ke posisi kursor
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        ghostRect.anchoredPosition = localPoint;

        slotUI.SetHighlight(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostRect == null) return;
        ghostRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGhost != null) Destroy(dragGhost);
        slotUI.SetHighlight(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragSource = eventData.pointerDrag?.GetComponent<ItemDragHandler>();
        if (dragSource == null) return;

        InventoryManager.Instance.SwapSlots(dragSource.slotUI.SlotIndex, slotUI.SlotIndex);
    }
}