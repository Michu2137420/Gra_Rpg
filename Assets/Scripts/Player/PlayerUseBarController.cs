using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUseBarController : MonoBehaviour
{
    [SerializeField] private Transform PlayerUseBarItemStoreTransform;
    public GameObject PlayerUseBarInventory;
    public GameObject slotPrefab;
    public int numberOfSlots;
    public List<InventoryItemDatabes.Item> currentyDisplayedForUseBarItems = new List<InventoryItemDatabes.Item>();
    private List<Slot> slots = new List<Slot>();

    // Start is called before the first frame update
    void Start()
    {
        PlayerUseBarInventory =transform.Find("PlayerUseBarPlace").gameObject;
        PlayerUseBarInventory.SetActive(true);
        CreateGridsInChest();
        DisplayItemsInUseBarInventory(currentyDisplayedForUseBarItems);
    }
    void OnDisable()
    {
        // Upewnij się, że nie tracimy referencji
        if (PlayerUseBarInventory == null)
        {
            PlayerUseBarInventory = transform.Find("PlayerUseBarPlace").gameObject;
        }
    }
    public void CreateGridsInChest()
    {
        int i = 0;
        while (i < numberOfSlots)
        {
            GameObject slot = Instantiate(slotPrefab, PlayerUseBarItemStoreTransform);
            slots.Add(slot.GetComponent<Slot>());
            i++;
        }
    }
    public void DisplayItemsInUseBarInventory(List<InventoryItemDatabes.Item> items)
    {
        currentyDisplayedForUseBarItems = new List<InventoryItemDatabes.Item>();


        int count = Mathf.Min(items.Count, numberOfSlots);

        for (int i = 0; i < count; i++)
        {
            currentyDisplayedForUseBarItems.Add(items[i]);
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < currentyDisplayedForUseBarItems.Count)
            {
                if (slots[i] != null)
                {
                    Transform itemIconTransform = slots[i].transform.Find("ItemIcon");
                    if (itemIconTransform != null)
                    {
                        Image iconImage = itemIconTransform.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.sprite = currentyDisplayedForUseBarItems[i]?.itemIcon;
                            slots[i].SetItem(currentyDisplayedForUseBarItems[i]);
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
            //else
            //{
            //    // Ukryj slot je�li nie ma ju� item�w
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
