using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInventoryController : MonoBehaviour
{
    public GameObject buildingInventory { get; private set; }
    public BuildingStation buildingStation;
    [field: SerializeField] public int numberOfSlots { get; private set; } = 10;
    [SerializeField] private GameObject buildingInventorySlotPrefab;
    [SerializeField] private Transform buildingInventoryPlaceTransform;
    [field: SerializeField] public GameObject buildingItemPrefab { get;  set; }


    private List<InventoryItemDatabes.Item> currentyDisplayedItems = new List<InventoryItemDatabes.Item>();
    [field: SerializeField] private List<BuildingInventorySlot> buildingSlots = new List<BuildingInventorySlot>();


    public int currentPageIdInBuildingManager { get; private set; }

    void Start()
    {
        buildingInventory = gameObject;
        buildingInventory.SetActive(false);
        currentPageIdInBuildingManager = buildingStation.currentPageIdInBuildingStation;
        DisplaySlotsInBuildingMenu();
    }

    private void DisplaySlotsInBuildingMenu()
    {
        int i = 0;
        while (i < numberOfSlots)
        {
            GameObject slot = Instantiate(buildingInventorySlotPrefab, buildingInventoryPlaceTransform);
            buildingSlots.Add(slot.GetComponent<BuildingInventorySlot>());
            i++;
        }
    }

    public void DisplayItemsInInventory(List<InventoryItemDatabes.Item> items)
    {
        //Debug.Log($"obecna lista przedmiotów: {items}" );
        
        currentyDisplayedItems = items;

        for (int i = 0; i < buildingSlots.Count; i++)
        {
            //if (buildingSlots[i] == null) continue;

            if (i < currentyDisplayedItems.Count && currentyDisplayedItems[i] != null)
            {
                //Debug.Log($"Wyœwietlanie przedmiotu w slocie {i}: {currentyDisplayedItems[i].itemName}");
                // Wyœwietl przedmiot
                InventoryItemDatabes.Item item = currentyDisplayedItems[i];
                
                DisplayItemInSlot(buildingSlots[i], item, i);
            }
            else
            {
                // Wyczyœæ slot jeœli nie ma przedmiotu lub iloœæ <= 0
                ClearSlot(buildingSlots[i]);
            }
            buildingSlots[i].SlotID = i;
        }
    }

    private void DisplayItemInSlot(BuildingInventorySlot slot, InventoryItemDatabes.Item item, int slotIndex)
    {
        Transform itemIconTransform = slot.transform.Find("BuildingItemIcon");
        if (itemIconTransform == null)
        {
            //Debug.LogError($"Nie znaleziono obiektu 'BuildingItemIcon' w slocie {slotIndex}");
            // Debug wszystkich dzieci dla diagnostyki
            Debug.Log($"Dzieci slotu {slotIndex}:");
            for (int i = 0; i < slot.transform.childCount; i++)
            {
                Debug.Log($"  - {slot.transform.GetChild(i).name}");
            }
            return;
        }

        // Ustaw ikonê przedmiotu
        Image iconImage = itemIconTransform.GetComponent<Image>();
        if (iconImage == null)
        {
            Debug.LogError($"Brak komponentu Image w BuildingItemIcon (slot {slotIndex})");
            return;
        }

        // WA¯NE: Ustaw sprite i poka¿ ItemIcon
        iconImage.sprite = item.itemIcon;
        itemIconTransform.gameObject.SetActive(true);

        // Debug dla weryfikacji
        //Debug.Log($"Ustawiono sprite dla slotu {slotIndex}: {item.itemIcon?.name}");

        slot.SetItem(item);
    }

    private void ClearSlot(BuildingInventorySlot slot)
    {
        // POPRAWKA: U¿yj poprawnej nazwy
        Transform itemIconTransform = slot.transform.Find("BuildingItemIcon");
        if (itemIconTransform != null)
        {
            Image iconImage = itemIconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = null;
            }
            itemIconTransform.gameObject.SetActive(false);
            
        }

        slot.SetItem(null);
    }
}