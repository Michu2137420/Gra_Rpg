using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BuildingInventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<GameObject> OnItemChoosenFromBuildingMenu;
    private BuildingInventoryController buildingInventoryController;

    public int SlotID;
    private InventoryItemDatabes.Item _item;
    public Sprite ItemIcon;

    void Start()
    {
         buildingInventoryController =FindInParents<BuildingInventoryController>(gameObject);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"przeslano {buildingInventoryController.buildingItemPrefab}");
        OnItemChoosenFromBuildingMenu.Invoke(buildingInventoryController.buildingItemPrefab);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Tutaj mo¿esz dodaæ hover effect
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Tutaj mo¿esz usun¹æ hover effect
    }
    public void SetItem(InventoryItemDatabes.Item item)
    {
        this._item = item;
       // Debug.Log($"Ustawiono item w slocie {SlotID}: {item?.itemName ?? "null"}");

        if (item != null && item.itemIcon != null)
        {
            ItemIcon = item.itemIcon;
            //Debug.Log($"Ustawiono item w slocie {SlotID}: {item.itemName}");
        }
        else
        {
            ItemIcon = null;
            //Debug.Log($"Wyczyszczono slot {SlotID}");

            // Upewnij siê, ¿e ikona jest wyczyszczona
            Transform itemIconTransform = transform.Find("BuildingItemIcon");
            if (itemIconTransform != null)
            {
                Image iconImage = itemIconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = null;
                }
                itemIconTransform.gameObject.SetActive(false);
            }
        }
    }
    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();
        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}