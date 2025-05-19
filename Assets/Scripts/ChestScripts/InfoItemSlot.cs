using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class InfoItemSlot : MonoBehaviour
{
    public static InfoItemSlot instance;
    private Image itemImageInDescription;
    public TextMeshProUGUI itemNameText;
    public Color32 normalcolor;
    public Color32 enterColor;
    public Sprite itemIcon;

    public void DisplayItemIconWhenClicked(InventoryItemDatabes.Item item)
    {
        // SprawdŸ, czy przekazany item nie jest nullem
        if (item == null)
        {
            Debug.LogWarning("Przekazany item jest nullem w DisplayItemIconWhenClicked.");
            return;
        }

        // Przypisz wartoœci do obiektów potomnych
        Transform itemInfoImage = transform.Find("InfoItemSlot/InfoItemImage");
        Transform itemInfoDescription = transform.Find("InfoItemDescriptionPanel/InfoItemDescription");

        if (itemInfoImage != null)
        {
            Image imageComponent = itemInfoImage.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = item.itemIcon != null ? item.itemIcon : null;
            }
            else
            {
                Debug.LogWarning("Brak komponentu Image w InfoItemImage.");
            }
        }
        else
        {
            Debug.LogWarning("Nie znaleziono obiektu InfoItemImage.");
        }

        if (itemInfoDescription != null)
        {
            TextMeshProUGUI textComponent = itemInfoDescription.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = !string.IsNullOrEmpty(item.itemName) ? item.itemName : "Brak nazwy";
            }
            else
            {
                Debug.LogWarning("Brak komponentu TextMeshProUGUI w InfoItemDescription.");
            }
        }
        else
        {
            Debug.LogWarning("Nie znaleziono obiektu InfoItemDescription.");
        }
    }

    void Start()
    {
        itemImageInDescription = GetComponent<Image>();
        itemImageInDescription.color = normalcolor;
    }
    private void Awake()
    {
        instance=this;
    }   
}
