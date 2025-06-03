using UnityEngine;

public class Building : MonoBehaviour
{

    public bool placed { get; private set; } = false;
    public BoundsInt area;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CanBePlaced()
    {
        Vector3Int positionInt = GridBuildingSystem.Instance.gridLayout.WorldToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;
        if(GridBuildingSystem.Instance.CanTakeArea(areaTemp))
        {
            return true;
        }
        return false;
    }
    public void Place()
    {
       Vector3Int vector3Int = GridBuildingSystem.Instance.gridLayout.WorldToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = vector3Int;
        placed = true;
        GridBuildingSystem.Instance.TakeArea(areaTemp);
    }


}
