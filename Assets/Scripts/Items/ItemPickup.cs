using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(itemData);
            Destroy(gameObject);
        }
    }
}
