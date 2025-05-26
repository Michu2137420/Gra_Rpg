using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        currentyDisplayedForUseBarItems = items;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;

            if (i < currentyDisplayedForUseBarItems.Count && currentyDisplayedForUseBarItems[i] != null)
            {
                // Wyświetl przedmiot
                InventoryItemDatabes.Item item = currentyDisplayedForUseBarItems[i];
                DisplayItemInSlot(slots[i], item, i);
            }
            else
            {
                // Wyczyść slot jeśli nie ma przedmiotu
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

        // Ustaw ikonę przedmiotu
        Image iconImage = itemIconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogError($"Brak komponentu Image w ItemIcon (slot {slotIndex})");
            return;
        }

        iconImage.sprite = item.itemIcon;
        slot.SetItem(item);
        itemIconTransform.gameObject.SetActive(true);

        // Obsługa tekstu z ilością przedmiotów
        UpdateAmountText(itemIconTransform, item, slotIndex);
    }

    private void UpdateAmountText(Transform itemIconTransform, InventoryItemDatabes.Item item, int slotIndex)
    {
        // Znajdź AmountOfItemText
        Transform amountOfItemTextTransform = itemIconTransform.Find("AmountOfItemText");
        if (amountOfItemTextTransform == null)
        {
            Debug.LogWarning($"Nie znaleziono obiektu 'AmountOfItemText' w slocie {slotIndex}");
            return;
        }

        // Znajdź Text wewnątrz AmountOfItemText
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

        // Logika wyświetlania ilości
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

            // Wyczyść tekst ilości
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
