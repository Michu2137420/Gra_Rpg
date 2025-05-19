using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public ChestInventoryController chestInventoryController;
    public int chestID;
    public List<InventoryItemDatabes.Item> initialItems;
    public  List<InventoryItemDatabes.Item> itemsInChest;

    private static int nextChestID = 0; 

    private bool playerInRange = false;
    private PlayerController player;

    void Awake()
    {
        // Automatyczne nadanie unikalnego ID
        chestID = nextChestID++;

        // Inicjalizacja listy o wielkoœci równej liczbie slotów w skrzyni, najpierw pierwotne itemy, reszta null
        if (chestInventoryController != null)
        {
            itemsInChest = new List<InventoryItemDatabes.Item>(chestInventoryController.numberOfSlots);
            int slots = chestInventoryController.numberOfSlots;
            int initialCount = initialItems != null ? initialItems.Count : 0;

            // Dodaj pierwotne itemy
            for (int i = 0; i < slots; i++)
            {
                if (initialItems != null && i < initialItems.Count)
                {
                    itemsInChest.Add(initialItems[i]);
                }
                else
                {
                    itemsInChest.Add(null);
                }
            }
        }
        else
        {
            itemsInChest = new List<InventoryItemDatabes.Item>();
        }
    }

    void Start()
    {
        ManagerForListOfChestInventory.instance.AddChestItemsToList(itemsInChest, chestID);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            player = collision.GetComponent<PlayerController>();
            Collider2D chestCollider = GetComponent<BoxCollider2D>();

            if (collision.IsTouching(chestCollider))
            {
                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            playerInRange = false;

            if (chestInventoryController != null && chestInventoryController.isOpen)
            {
                chestInventoryController.chestInventory.SetActive(false);
                chestInventoryController.isOpen = false;
            }
        }
    }
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // itemsInChest = ManagerForChestInventory.instance.DowloandListOfItemsFromGlobalListOfLists(chestID);
            chestInventoryController.chestInventory.SetActive(!chestInventoryController.chestInventory.activeInHierarchy);
            chestInventoryController.isOpen = !chestInventoryController.isOpen;
            chestInventoryController.DisplayItemsInInventory(itemsInChest);
            foreach (var slot in FindObjectsByType<Slot>(FindObjectsSortMode.None))
            {
                slot.SetCurrentChestItems(itemsInChest);
            }
        }
    }
}

