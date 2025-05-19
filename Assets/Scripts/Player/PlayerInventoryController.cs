using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private Transform playerItemStoreTransform;
    public GameObject playerInventory;
    public GameObject slotPrefab;
    public bool isOpen;
    public int numberOfSlots;
    public List<InventoryItemDatabes.Item> currentyDisplayedItems = new List<InventoryItemDatabes.Item>();
    private List<Slot> slots = new List<Slot>();

    void Start()
    {
        playerInventory = gameObject;
        //Debug.Log("ChestInventory: " + chestInventory);
        playerInventory.SetActive(false);
        isOpen = false;
        CreateGridsInChest();
    }
 
    public void CreateGridsInChest()
    {
        int i = 0;
        //Debug.Log("Parent: " + chestItemStoreTransform);
        while (i < numberOfSlots)
        {
            GameObject slot = Instantiate(slotPrefab, playerItemStoreTransform);
            slots.Add(slot.GetComponent<Slot>());
            i++;
        }
    }
    //public void InitItemInChest()
    //{
    //    // foreach()
    //}
    public void DisplayItemsInInventory(List<InventoryItemDatabes.Item> items)
    {
        currentyDisplayedItems = items;

        for (int i = 0; i < slots.Count; i++)
        {
            // Jeœli mamy item do pokazania
            if (i < currentyDisplayedItems.Count)
            {
                if (slots[i] != null)
                {
                    // Spróbuj znaleŸæ "ItemIcon" w slocie
                    Transform itemIconTransform = slots[i].transform.Find("ItemIcon");
                    if (itemIconTransform != null)
                    {
                        Image iconImage = itemIconTransform.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.sprite = currentyDisplayedItems[i]?.itemIcon;
                            slots[i].SetItem(currentyDisplayedItems[i]);
                            itemIconTransform.gameObject.SetActive(true);
                        }
                        else
                        {
                            Debug.LogError($"Brak komponentu Image w ItemIcon (slot {i})");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Nie znaleziono obiektu 'ItemIcon' w slocie {i}");
                    }
                    slots[i].slotID = i;
                }
                else
                {
                    Debug.LogError($"Slot {i} jest null");
                }
            }
            // Jeœli nie mamy ju¿ itemów, ukryj slot
            //else
            //{
            //    Transform itemIconTransform = slots[i].transform.Find("ItemIcon");
            //    if (itemIconTransform != null)
            //    {
            //        Image iconImage = itemIconTransform.GetComponent<Image>();
            //        if (iconImage != null)
            //        {
            //            iconImage.sprite = null;
            //            itemIconTransform.gameObject.SetActive(false);
            //        }
            //    }
            //}
        }
    }






}
