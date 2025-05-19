using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ManagerForListOfChestInventory : MonoBehaviour
{
    public static ManagerForListOfChestInventory instance;
    public  List<ChestItems> globalChestItemsList = new List<ChestItems>();

    public void Awake()
    {
        instance=this;

    }

    // Dodaje now� list� przedmiot�w do globalnej listy, przypisuj�c jej podane ID
    public void AddChestItemsToList( List<InventoryItemDatabes.Item> items, int chestId)
    {
        ChestItems chestItems = new ChestItems(chestId, items);
        globalChestItemsList.Add(chestItems);
    }
    // Pobiera list� przedmiot�w z globalnej listy na podstawie podanego ID skrzynki
    public List<InventoryItemDatabes.Item> DowloandListOfItemsFromGlobalListOfLists(int chestID)
    {
        // Sprawdza, czy lista przedmiot�w dla danej skrzynki istnieje w globalnej li�cie
        ChestItems foundChest = globalChestItemsList.Find(x => x.chestId == chestID);
        // Je�li tak, zwraca list� przedmiot�w
        if (foundChest != null)
        {
           // Debug.Log("Found chest with ID: " + foundChest.chestId);
            return new List<InventoryItemDatabes.Item>(foundChest.itemsInChest);
        }
        return new List<InventoryItemDatabes.Item>();
    }
}
[System.Serializable]
public class ChestItems
{
    public int chestId;
    public List<InventoryItemDatabes.Item> itemsInChest;

    public ChestItems(int chestId, List<InventoryItemDatabes.Item> items)
    {
        this.chestId = chestId;
        this.itemsInChest = items;
    }
}
