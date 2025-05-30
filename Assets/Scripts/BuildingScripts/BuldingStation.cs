using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BuldingStation : MonoBehaviour
{
    private bool playerInRange { get; set; } = false;
    [SerializeField] private BuildingInventoryController buildingInventoryController;
    [SerializeField] private List<int> buildingTabIds = new List<int>();
    [field:SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage1 = new List<InventoryItemDatabes.Item>();
    [field:SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage2 = new List<InventoryItemDatabes.Item>();
    [field:SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage3 = new List<InventoryItemDatabes.Item>();
    [field:SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage4 = new List<InventoryItemDatabes.Item>();
    [field:SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage5 = new List<InventoryItemDatabes.Item>();
    [field:SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage6 = new List<InventoryItemDatabes.Item>();
    [SerializeField] private int amountOfPagesInBuildingMenu;

    private void Start()
    {
        InitializeIdForPages();
        UploadPagesListToManager();
        InitializeAllBuildingTabs();

    }
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            buildingInventoryController.buildingInventory.SetActive(!buildingInventoryController.buildingInventory.activeInHierarchy);
            buildingInventoryController.DisplayItemsInInventory(buldingItemsToBuildPage1);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInRange = false;
        if(buildingInventoryController!=null)
        {if (buildingInventoryController.buildingInventory.activeInHierarchy)
            {
                buildingInventoryController.buildingInventory.SetActive(false);
            }
        }

    }
    private void InitializeIdForPages()
    {
        buildingTabIds.Clear();
        for (int i = 0; i < amountOfPagesInBuildingMenu; i++)
        {
            buildingTabIds.Add(i);

        }

    }
    private void UploadPagesListToManager()
    {
        // Lista wszystkich list itemów do budowy
        var allPages = new List<List<InventoryItemDatabes.Item>> {
            buldingItemsToBuildPage1,
            buldingItemsToBuildPage2,
            buldingItemsToBuildPage3,
            buldingItemsToBuildPage4,
            buldingItemsToBuildPage5,
            buldingItemsToBuildPage6
        };
        for (int i = 0; i < buildingTabIds.Count && i < allPages.Count; i++)
        {
            ManagerForListsOfConentForPagesInBuildingMenu.Instance.AddBuildingMenuItemsToList(allPages[i], buildingTabIds[i]);
        }
    }
    private void InitializeItemsInList(List<InventoryItemDatabes.Item> listOfItemsToBuild)
    {
        // Inicjalizuje przekazan¹ listê itemów do budowy (np. dla danej zak³adki)
        if (listOfItemsToBuild == null)
            return;

        if (buildingInventoryController != null)
        {
            int slots = buildingInventoryController.numberOfSlots;

            // Upewnij siê, ¿e lista ma dok³adnie tyle elementów co slotów, wype³niaj¹c puste miejsca nullami
            if (listOfItemsToBuild.Count < slots)
            {
                int toAdd = slots - listOfItemsToBuild.Count;
                for (int i = 0; i < toAdd; i++)
                {
                    listOfItemsToBuild.Add(null);
                }
            }
            else if (listOfItemsToBuild.Count > slots)
            {
                listOfItemsToBuild.RemoveRange(slots, listOfItemsToBuild.Count - slots);
            }
        }
        else
        {
            Debug.LogWarning("BuldingInventoryController is null!");
        }
    }

    // Przyk³ad u¿ycia dla wszystkich zak³adek (np. w Start lub innej metodzie inicjalizuj¹cej):
    private void InitializeAllBuildingTabs()
    {
        InitializeItemsInList(buldingItemsToBuildPage1);
        InitializeItemsInList(buldingItemsToBuildPage2);
        InitializeItemsInList(buldingItemsToBuildPage3);
        InitializeItemsInList(buldingItemsToBuildPage4);
        InitializeItemsInList(buldingItemsToBuildPage5);
        InitializeItemsInList(buldingItemsToBuildPage6);
    }
}



