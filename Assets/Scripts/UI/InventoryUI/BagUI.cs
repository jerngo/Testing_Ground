using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : MonoBehaviour
{
    public GameObject bagPanel;
    public List<Image> slotImages; // isi 6 slot di inspector

    bool isOpen = false;

    void Start()
    {
        bagPanel.SetActive(false);
    }

    //public void ToggleBag()
    //{
    //    isOpen = !isOpen;
    //    bagPanel.SetActive(isOpen);

    //    if (isOpen)
    //    {
    //        RefreshUI();
    //    }
    //}

    //void RefreshUI()
    //{
    //    var items = InventoryManager.Instance.items;

    //    for (int i = 0; i < slotImages.Count; i++)
    //    {
    //        if (i < items.Count && items[i] != null)
    //        {
    //            slotImages[i].sprite = items[i].icon;

    //            Color c = slotImages[i].color;
    //            c.a = 1f;
    //            slotImages[i].color = c;
    //        }
    //        else
    //        {
    //            Color c = slotImages[i].color;
    //            c.a = 0f;
    //            slotImages[i].color = c;
    //        }
    //    }
    //}
}
