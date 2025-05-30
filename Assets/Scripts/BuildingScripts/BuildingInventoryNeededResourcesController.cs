using System.Collections.Generic;
using UnityEngine;

public class BuildingInventoryNeededResourcesController : MonoBehaviour
{
    public GameObject buildingInventoryResourcesNeeded { get; private set; }
    [field: SerializeField] private int numberOfSlots = 7;
    [SerializeField] private GameObject buildingInventoryResourcesNeededSlotPrefab;
    [SerializeField] private Transform buildingInventoryResourcesNeededPlaceTransform;
    [field: SerializeField] private List<BuildingInventorySlot> buildingSlots = new List<BuildingInventorySlot>();
    void Start()
    {
        buildingInventoryResourcesNeeded = gameObject;
        //buildingInventoryResourcesNeeded.SetActive(false);
        DisplaySlotsInBuildingMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void DisplaySlotsInBuildingMenu()
    {
        int i = 0;
        //Debug.Log("Parent: " + chestItemStoreTransform);
        while (i < numberOfSlots)
        {
            GameObject slot = Instantiate(buildingInventoryResourcesNeededSlotPrefab, buildingInventoryResourcesNeededPlaceTransform);
            buildingSlots.Add(slot.GetComponent<BuildingInventorySlot>());
            i++;
        }
    }
}
