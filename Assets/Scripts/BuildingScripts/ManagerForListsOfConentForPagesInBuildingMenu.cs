using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ManagerForListsOfConentForPagesInBuildingMenu : MonoBehaviour
{
    public static ManagerForListsOfConentForPagesInBuildingMenu Instance;
    [field: SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage1 = new List<InventoryItemDatabes.Item>();
    [field: SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage2 = new List<InventoryItemDatabes.Item>();
    [field: SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage3 = new List<InventoryItemDatabes.Item>();
    [field: SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage4 = new List<InventoryItemDatabes.Item>();
    [field: SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage5 = new List<InventoryItemDatabes.Item>();
    [field: SerializeField] List<InventoryItemDatabes.Item> buldingItemsToBuildPage6 = new List<InventoryItemDatabes.Item>();
    [field: SerializeField] List<BuildingMenuContent> globalListOfListsOfContentsInPagesInBuildingMenu = new List<BuildingMenuContent>();
    [SerializeField] private List<int> buildingTabIds = new List<int>();
    [SerializeField] private BuildingInventoryController buildingInventoryController;
    [field:SerializeField]private int amountOfPagesInBuildingMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        InitializeIdForPages();
        UploadPagesListToManager();
    }

    // Dodaje now¹ listê przedmiotów do globalnej listy, przypisuj¹c jej podane ID
    public void AddBuildingMenuItemsToList(List<InventoryItemDatabes.Item> listOfItemsToBuild, int pageID)
    {
        BuildingMenuContent buldingItemsToBuild = new BuildingMenuContent(pageID, listOfItemsToBuild);
        globalListOfListsOfContentsInPagesInBuildingMenu.Add(buldingItemsToBuild);
    }
    // Pobiera listê przedmiotów z globalnej listy na podstawie podanego ID skrzynki
    public List<InventoryItemDatabes.Item> DowloandListOfItemsFromGlobalListOfLists(int pageID)
    {
        // Sprawdza, czy lista przedmiotów dla danej skrzynki istnieje w globalnej liœcie
        BuildingMenuContent foundPage = globalListOfListsOfContentsInPagesInBuildingMenu.Find(x => x.pageID == pageID);
        // Jeœli tak, zwraca listê przedmiotów
        if (foundPage != null)
        {
            // Debug.Log("Found chest with ID: " + foundChest.chestId);
            return new List<InventoryItemDatabes.Item>(foundPage.buildingsToBuild);
        }
        return new List<InventoryItemDatabes.Item>();
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
           AddBuildingMenuItemsToList(allPages[i], buildingTabIds[i]);
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
[System.Serializable]
public class BuildingMenuContent
{
    public int pageID;
    public List<InventoryItemDatabes.Item> buildingsToBuild;

    public BuildingMenuContent(int pageID, List<InventoryItemDatabes.Item> items)
    {
        this.pageID = pageID;
        this.buildingsToBuild = items;
    }
}
