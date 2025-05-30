using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ManagerForListsOfConentForPagesInBuildingMenu : MonoBehaviour
{
    public static ManagerForListsOfConentForPagesInBuildingMenu Instance;
    [field:SerializeField]List<BuildingMenuContent> globalListOfListsOfContentsInPagesInBuildingMenu = new List<BuildingMenuContent>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
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
