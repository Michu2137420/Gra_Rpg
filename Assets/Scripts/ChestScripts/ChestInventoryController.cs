using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChestInventoryController : MonoBehaviour
{
    [SerializeField] private Transform chestItemStoreTransform;
    public GameObject chestInventory;
    public GameObject slotPrefab;
    public bool isOpen;
    public int numberOfSlots;
    public List<InventoryItemDatabes.Item> currentyDisplayedItems = new List<InventoryItemDatabes.Item>();
    public TextMeshProUGUI itemNameText;
    private List<Slot> slots = new List<Slot>();

    // Start is called before the first frame update
    void Start()
    {
        chestInventory = gameObject;
        //Debug.Log("ChestInventory: " + chestInventory);
        chestInventory.SetActive(false);
        isOpen = false;
        CreateGridsInChest();
    }

    public void CreateGridsInChest()
    {
        int i = 0;
        //Debug.Log("Parent: " + chestItemStoreTransform);
        while (i < numberOfSlots)
        {
            GameObject slot = Instantiate(slotPrefab, chestItemStoreTransform);
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

            if (i < currentyDisplayedItems.Count && currentyDisplayedItems[i] != null)
            {
                // Wyœwietl przedmiot
                InventoryItemDatabes.Item item = currentyDisplayedItems[i];
                DisplayItemInSlot(slots[i], item, i);
            }
            else
            {
                // Wyczyœæ slot jeœli nie ma przedmiotu
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

        // Ustaw ikonê przedmiotu
        Image iconImage = itemIconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogError($"Brak komponentu Image w ItemIcon (slot {slotIndex})");
            return;
        }

        iconImage.sprite = item.itemIcon;
        slot.SetItem(item);
        itemIconTransform.gameObject.SetActive(true);

        // Obs³uga tekstu z iloœci¹ przedmiotów
        UpdateAmountText(itemIconTransform, item, slotIndex);
    }

    private void UpdateAmountText(Transform itemIconTransform, InventoryItemDatabes.Item item, int slotIndex)
    {
        // ZnajdŸ AmountOfItemText
        Transform amountOfItemTextTransform = itemIconTransform.Find("AmountOfItemText");
        if (amountOfItemTextTransform == null)
        {
            Debug.LogWarning($"Nie znaleziono obiektu 'AmountOfItemText' w slocie {slotIndex}");
            return;
        }

        // ZnajdŸ Text wewn¹trz AmountOfItemText
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

        // Logika wyœwietlania iloœci
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
            //itemIconTransform.gameObject.SetActive(false);

            // Wyczyœæ tekst iloœci
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
