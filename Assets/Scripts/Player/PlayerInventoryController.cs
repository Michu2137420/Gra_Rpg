using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public void DisplayItemsInInventory(List<InventoryItemDatabes.Item> items)
    {
        currentyDisplayedItems = items;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;

            if (i < currentyDisplayedItems.Count && currentyDisplayedItems[i] != null && currentyDisplayedItems[i].itemAmount >= 0)
            {
                // Wy�wietl przedmiot
                InventoryItemDatabes.Item item = currentyDisplayedItems[i];
                DisplayItemInSlot(slots[i], item, i);
            }
            else
            {
                // Wyczy�� slot je�li nie ma przedmiotu lub ilo�� <= 0
                ClearSlot(slots[i]);
            }
            slots[i].SlotID = i;
        }
    }

    private void DisplayItemInSlot(Slot slot, InventoryItemDatabes.Item item, int slotIndex)
    {
        Transform itemIconTransform = slot.transform.Find("ItemIcon");
        if (itemIconTransform == null)
        {
            Debug.LogError($"Nie znaleziono obiektu 'ItemIcon' w slocie {slotIndex}");
            return;
        }

        // Ustaw ikon� przedmiotu
        Image iconImage = itemIconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogError($"Brak komponentu Image w ItemIcon (slot {slotIndex})");
            return;
        }

        // WA�NE: Ustaw sprite i poka� ItemIcon
        iconImage.sprite = item.itemIcon;
        itemIconTransform.gameObject.SetActive(true);

        slot.SetItem(item);

        // Obs�uga tekstu z ilo�ci� przedmiot�w
        UpdateAmountText(itemIconTransform, item, slotIndex);
    }

    private void UpdateAmountText(Transform itemIconTransform, InventoryItemDatabes.Item item, int slotIndex)
    {
        // Znajd� AmountOfItemText
        Transform amountOfItemTextTransform = itemIconTransform.Find("AmountOfItemText");
        if (amountOfItemTextTransform == null)
        {
            Debug.LogWarning($"Nie znaleziono obiektu 'AmountOfItemText' w slocie {slotIndex}");
            return;
        }

        // Znajd� Text wewn�trz AmountOfItemText
        Transform textTransform = amountOfItemTextTransform.Find("ItemAmountText");
        if (textTransform == null)
        {
            Debug.LogWarning($"Nie znaleziono obiektu 'Text' w AmountOfItemText w slocie {slotIndex}");
            return;
        }

        TextMeshProUGUI amountText = textTransform.GetComponent<TextMeshProUGUI>();
        if (amountText == null)
        {
            Debug.LogWarning($"Brak komponentu TextMeshProUGUI w Text w slocie {slotIndex}");
            return;
        }

        // Logika wy�wietlania ilo�ci
        bool shouldShowAmount = item.isStackable && item.itemAmount > 1;

        if (shouldShowAmount)
        {
            amountText.text = item.itemAmount.ToString();
            amountText.gameObject.SetActive(true);
            amountOfItemTextTransform.gameObject.SetActive(true);
        }
        else
        {
            amountText.text = "";
            amountText.gameObject.SetActive(false);
            amountOfItemTextTransform.gameObject.SetActive(false);
        }

    }
    private void ClearSlot(Slot slot)
    {
        Transform itemIconTransform = slot.transform.Find("ItemIcon");
        if (itemIconTransform != null)
        {
            // WA�NE: Wyczy�� sprite i ukryj ItemIcon
            Image iconImage = itemIconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = null;
            }
            itemIconTransform.gameObject.SetActive(false);

            // Wyczy�� tekst ilo�ci
            Transform amountOfItemTextTransform = itemIconTransform.Find("AmountOfItemText");
            if (amountOfItemTextTransform != null)
            {
                amountOfItemTextTransform.gameObject.SetActive(false);

                Transform textTransform = amountOfItemTextTransform.Find("ItemAmountText");
                if (textTransform != null)
                {
                    TextMeshProUGUI amountText = textTransform.GetComponent<TextMeshProUGUI>();
                    if (amountText != null)
                    {
                        amountText.text = "";
                    }
                }
            }
        }

        slot.SetItem(null);
    }






}
