using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BuildingStation : MonoBehaviour
{
    private bool playerInRange { get; set; } = false;
    [SerializeField] private BuildingInventoryController buildingInventoryController;
    [field: SerializeField] public int currentPageIdInBuildingStation { get; set; }

    private void Start()
    {
    }
    private void Update()
    {
        
    }
    public void OpeningBuildingInventory()
    {
        List<InventoryItemDatabes.Item> tempList = OpeningHelpFunction(currentPageIdInBuildingStation);
        Debug.Log($"Otwieranie inwentarza budynku z ID strony: {currentPageIdInBuildingStation}");
        buildingInventoryController.buildingInventory.SetActive(true);
        buildingInventoryController.DisplayItemsInInventory(tempList);
        InventoryGuiManager.howManyOpen=1;

    }
    private List<InventoryItemDatabes.Item> OpeningHelpFunction(int currentPageID)
    {
        return  ManagerForListsOfConentForPagesInBuildingMenu.Instance.DowloandListOfItemsFromGlobalListOfLists(currentPageID);
 
    }
    public void CloseBuildingInventory()
    {
        if (buildingInventoryController.buildingInventory.activeInHierarchy)
        {
            buildingInventoryController.buildingInventory.SetActive(false);
            InventoryGuiManager.howManyOpen=0;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnEnable()
    {
        BuildingInventorySlot.OnClickCloseBuildingInventory += CloseBuildingInventory;
        GridBuildingSystem.OnClickOpenBuildingInventory += OpeningBuildingInventory;
    }
    private void OnDisable()
    {
        BuildingInventorySlot.OnClickCloseBuildingInventory -= CloseBuildingInventory;
        GridBuildingSystem.OnClickOpenBuildingInventory -= OpeningBuildingInventory;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInRange = false;
        if(buildingInventoryController!=null)
        {if (buildingInventoryController.buildingInventory.activeInHierarchy)
            {
                CloseBuildingInventory();
            }
        }

    }
  


}



