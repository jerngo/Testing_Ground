using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public string chestID;

    public Animator anim;

    [Header("Item")]
    public ItemData itemToGive;
    public GameObject itemPrefab;
    public Transform spawnPoint;

    // FIELD
    bool isOpened;

    // PROPERTY
    public bool IsOpened => isOpened;

    public void Interact(PlayerController player)
    {
        if (isOpened)
            return;

        OpenChest();
    }

    void OpenChest()
    {
        isOpened = true;

        if (anim != null)
            anim.SetBool("IsOpen", true);

        SpawnItem();
    }

    // Dipakai save/load system
    public void SetOpened()
    {
        isOpened = true;

        if (anim != null)
            anim.SetBool("IsOpen", true);
    }

    void SpawnItem()
    {
        if (
            itemPrefab == null ||
            spawnPoint == null ||
            itemToGive == null
        )
        {
            Debug.LogError("Chest setup belum lengkap!");
            return;
        }

        GameObject obj =
            Instantiate(
                itemPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

        ItemPickup pickup =
            obj.GetComponent<ItemPickup>();

        if (pickup != null)
        {
            pickup.itemData = itemToGive;
        }

        Rigidbody2D rb =
            obj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(
                new Vector2(0, 3f),
                ForceMode2D.Impulse
            );
        }
    }
}