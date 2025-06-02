using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Chest : MonoBehaviour
{
    public ChestInventoryController chestInventoryController;
    public int chestID;
    public List<InventoryItemDatabes.Item> chestInitialItems;
    public  List<InventoryItemDatabes.Item> itemsInChest;

    private static int nextChestID = 0; 

    private bool playerInRange = false;

    void Start()
    {
        ManagerForListOfChestInventory.instance.AddChestItemsToList(itemsInChest, chestID);
        chestID = nextChestID++;
        //DontDestroyOnLoad(this.gameObject);
        InitializeItemsInList();
    }
    private void Update()
    {
    }
    private void OnEnable()
    {
        InventoryGuiManager.openSecondInventory += OpenChestInventory;
        InventoryGuiManager.closeSecondInventory += CloseChestInventory;
    }
    private void OnDisable()
    {
        InventoryGuiManager.openSecondInventory -= OpenChestInventory;
        InventoryGuiManager.closeSecondInventory -= CloseChestInventory;
    }
    private void OpenChestInventory()
    {
        if (playerInRange)
        {
            chestInventoryController.chestInventory.SetActive(true);
            chestInventoryController.isOpen = !chestInventoryController.isOpen;
            chestInventoryController.DisplayItemsInInventory(itemsInChest);
        }
    }
    private void CloseChestInventory()
    {
        if (playerInRange)
        {
            chestInventoryController.chestInventory.SetActive(false);
            chestInventoryController.isOpen = !chestInventoryController.isOpen;
            chestInventoryController.DisplayItemsInInventory(itemsInChest);
        }
    }
    void NewIDForChest()
    {

    }
    private void InitializeItemsInList()
    {
        // Inicjalizacja listy o wielko�ci r�wnej liczbie slot�w w Inv
        if (chestInventoryController != null)
        {
            int slots = chestInventoryController.numberOfSlots;

            // Najpierw stw�rz pust� list� wype�nion� nullami
            itemsInChest = new List<InventoryItemDatabes.Item>(slots);
            for (int i = 0; i < slots; i++)
            {
                itemsInChest.Add(null);
            }

            // Teraz dodaj pocz�tkowe przedmioty u�ywaj�c AddItemToInventory
            if (chestInitialItems != null)
            {
                foreach (var initialItem in chestInitialItems)
                {
                    if (initialItem != null)
                    {
                        // U�ywamy AddItemToInventory aby zachowa� logik� stackowania
                        bool success = AddItemToInventory(initialItem);

                        if (!success)
                        {
                            Debug.LogWarning($"Nie uda�o si� doda� pocz�tkowego przedmiotu: {initialItem.itemName}. Inwentarz mo�e by� pe�ny.");
                        }
                    }
                }
            }
        }
        else
        {
            itemsInChest = new List<InventoryItemDatabes.Item>();
            Debug.LogWarning("PlayerInventoryController is null!");
        }
    }
    public bool AddItemToInventory(InventoryItemDatabes.Item newItem)
    {
        if (newItem == null)
            return false;

        // Je�li item jest stackowalny, spr�buj znale�� ju� istniej�cy taki sam i zwi�ksz ilo��
        if (newItem.isStackable)
        {
            for (int i = 0; i < itemsInChest.Count; i++)
            {
                var existingItem = itemsInChest[i];
                if (existingItem != null &&
                    existingItem.itemID == newItem.itemID &&
                    existingItem.isStackable)
                {
                    // Zwi�ksz ilo�� istniej�cego itemu
                    existingItem.itemAmount += newItem.itemAmount;

                    // Od�wie� wy�wietlanie inwentarza je�li jest otwarty
                    if (chestInventoryController.isOpen)
                    {
                        chestInventoryController.DisplayItemsInInventory(itemsInChest);
                    }


                    return true;
                }
            }
        }

        // Je�li nie stackowalny lub nie znaleziono stackowalnego, dodaj do pierwszego wolnego slotu
        for (int i = 0; i < itemsInChest.Count; i++)
        {
            if (itemsInChest[i] == null)
            {
                // Stw�rz now� kopi� itemu aby unikn�� problem�w z referencjami
                itemsInChest[i] = new InventoryItemDatabes.Item(
                    newItem.itemName,
                    newItem.itemDescription,
                    newItem.itemID,
                    newItem.itemValue,
                    newItem.itemAmount,
                    newItem.itemIcon,
                    newItem.itemType
                );
                itemsInChest[i].isStackable = newItem.isStackable;

                // Od�wie� wy�wietlanie inwentarza je�li jest otwarty
                if (chestInventoryController.isOpen)
                {
                    chestInventoryController.DisplayItemsInInventory(itemsInChest);
                }

                return true;
            }
        }

        // Brak miejsca w inwentarzu
        Debug.Log("Brak miejsca w inwentarzu!");
        return false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            Collider2D chestCollider = GetComponent<BoxCollider2D>();

            if (collision.IsTouching(chestCollider))
            {
                playerInRange = true;
            }
        }
    }


}

