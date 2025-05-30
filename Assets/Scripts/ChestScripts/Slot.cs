using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;

[RequireComponent(typeof(Image))]
public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Image TargetSlot;
    GameObject SlotGameobject;
    private GameObject _itemFrame;
    private Image _itemIconImage;
    public ChestInventoryController ChestInventoryController;
    public PlayerInventoryController PlayerInventoryController;
    public Color32 Normalcolor;
    public Color32 EnterColor;
    public Sprite ItemIcon;
    public int SlotID;
    private InventoryItemDatabes.Item _item;
    public bool dragOnSurfaces = true;

    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    private static int _slotFirst { get; set; }
    private static int _slotSecond { get; set; }

    // Events dla komunikacji z ChestTransferingItemsLogic
    public static event Action<Slot, Slot> OnItemChangeSlotInTheSameList;
    public static event Action<Slot, Slot> OnItemTransferBetweenInventories;
    public static event Action<Slot, PointerEventData> OnItemDropToInventory;

    //event do obs�ugi wyboru slotu na pasku skr�t�w obs�ugiwany w PlayerController 
    public static class HotbarEvents
    {
        public static Action<int> OnSlotSelected;
    }

    /// <summary>
    /// Inicjalizuje referencje do slotu, kontroler�w ekwipunku oraz ustawia kolor pocz�tkowy.
    /// </summary>
    void Start()
    {
        SlotGameobject = gameObject;
        TargetSlot = GetComponent<Image>();
        _itemIconImage = transform.Find("ItemIcon")?.GetComponent<Image>();
        //TargetSlot.color = Normalcolor;
        ChestInventoryController = FindInParents<ChestInventoryController>(gameObject);
        PlayerInventoryController = FindInParents<PlayerInventoryController>(gameObject);
        _itemFrame = transform.Find("ItemFrame")?.gameObject;
    }

    /// <summary>
    /// Obs�uguje wyb�r slotu na pasku skr�t�w po naci�ni�ciu odpowiedniego klawisza.
    /// </summary>
    void Update()
    {
        //logika do u�ycia przedmiotu z paska skr�t�w
        if (SlotID >= 0 && SlotID <= 5)
        {
            for (int i = 0; i < 6; i++)
            {
                // KeyCode.Alpha1 to 1, Alpha2 to 2, itd.
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    if (SlotID == i)
                    {
                        OnUseBarSlotSelected();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Subskrybuje event wyboru slotu na pasku skr�t�w.
    /// </summary>
    void OnEnable()
    {
        // Subskrybuj event
        HotbarEvents.OnSlotSelected += OnOtherSlotSelected;
    }

    /// <summary>
    /// Odsubskrybowuje event wyboru slotu na pasku skr�t�w.
    /// </summary>
    private void OnDisable()
    {
        // Odsubskrybuj event
        HotbarEvents.OnSlotSelected -= OnOtherSlotSelected;
    }

    /// <summary>
    /// Ukrywa ramk� zaznaczenia, je�li wybrano inny slot na pasku skr�t�w.
    /// </summary>
    private void OnOtherSlotSelected(int selectedSlotID)
    {
        if (selectedSlotID != SlotID && _itemFrame != null)
        {
            _itemFrame.SetActive(false);
        }
    }

    /// <summary>
    /// Obs�uguje logik� wyboru slotu na pasku skr�t�w (zaznaczenie, powiadomienie innych slot�w).
    /// </summary>
    private void OnUseBarSlotSelected()
    {
        if (_item != null)
        {
            Debug.Log($"Wybrano slot {SlotID + 1} na pasku skr�t�w: {_item.itemName}");

            // Powiadom inne sloty o wyborze
            HotbarEvents.OnSlotSelected?.Invoke(SlotID);

            // Zaznacz ten slot
            if (_itemFrame != null)
            {
                _itemFrame.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"Slot {SlotID + 1} na pasku skr�t�w jest pusty.");
            HotbarEvents.OnSlotSelected?.Invoke(-1);
        }
    }

    /// <summary>
    /// Wywo�ywana po klikni�ciu na slot - wy�wietla informacje o przedmiocie.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_item != null)
        {
            InfoItemSlot.instance.DisplayItemIconWhenClicked(_item);
        }
    }

    /// <summary>
    /// Przywraca kolor slotu po opuszczeniu kursora.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
    }

    /// <summary>
    /// Zmienia kolor slotu po najechaniu kursorem oraz ustawia indeksy slot�w do zamiany.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {

        // Ustaw slotSecond na ten slot, bo na niego naje�d�amy
        _slotSecond = SlotID;

        // Je�li przeci�gamy inny slot, ustaw slotFirst na slot przeci�gany
        if (eventData.pointerDrag != null && eventData.pointerDrag != gameObject)
        {
            Slot draggedSlot = eventData.pointerDrag.GetComponent<Slot>();
            if (draggedSlot != null)
            {
                _slotFirst = draggedSlot.SlotID;
            }
            else
            {
                _slotFirst = SlotID;
            }
        }
        else
        {
            _slotFirst = SlotID;
        }
    }



    /// <summary>
    /// Zwraca aktualn� list� przedmiot�w gracza bezpo�rednio z kontrolera
    /// </summary>
    public List<InventoryItemDatabes.Item> GetCurrentPlayerItems()
    {
        if (PlayerInventoryController != null)
        {
            return PlayerInventoryController.currentyDisplayedItems;
        }

        // Fallback - spr�buj znale�� PlayerController w scenie
        var playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            return playerController.PlayerItems;
        }

        Debug.LogWarning("Nie mo�na znale�� PlayerInventoryController lub PlayerController!");
        return null;
    }

    /// <summary>
    /// Zwraca aktualn� list� przedmiot�w skrzynki bezpo�rednio z kontrolera
    /// </summary>
    public List<InventoryItemDatabes.Item> GetCurrentChestItems()
    {
        if (ChestInventoryController != null)
        {
            return ChestInventoryController.currentyDisplayedItems;
        }

        Debug.LogWarning("Nie mo�na znale�� ChestInventoryController!");
        return null;
    }

    /// <summary>
    /// Rozpoczyna przeci�ganie przedmiotu - tworzy wizualn� reprezentacj� przeci�ganego obiektu.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_item == null || _item.itemIcon == null)
            return;

        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;
        m_DraggingIcon = new GameObject("DraggedIcon");
        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();
        image.sprite = _item.itemIcon;
        // Ustaw rozmiar przeci�ganego obrazka na 50x50 pikseli
        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        image.raycastTarget = false;

        m_DraggingPlane = dragOnSurfaces ? transform as RectTransform : canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    /// <summary>
    /// Aktualizuje pozycj� przeci�ganego obiektu podczas przeci�gania.
    /// </summary>
    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }

    /// <summary>
    /// Ustawia pozycj� przeci�ganego obiektu na podstawie pozycji kursora.
    /// </summary>
    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform is RectTransform)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

    /// <summary>
    /// Ko�czy przeci�ganie przedmiotu - wywo�uje odpowiednie eventy dla ChestTransferingItemsLogic.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        Transform pointerTransform = eventData.pointerEnter != null ? eventData.pointerEnter.transform : null;
        Slot targetSlot = null;
        while (pointerTransform != null && targetSlot == null)
        {
            targetSlot = pointerTransform.GetComponent<Slot>();
            pointerTransform = pointerTransform.parent;
        }

        if (targetSlot != null && targetSlot != this)
        {
            // Okre�l �r�d�owy slot (dragged) i docelowy (target)
            Slot draggedSlot = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<Slot>() : null;
            if (draggedSlot != null && draggedSlot != targetSlot)
            {
                // Przypadek: przeci�gamy mi�dzy r�nymi ekwipunkami
                if ((draggedSlot.PlayerInventoryController != null && targetSlot.ChestInventoryController != null) ||
                    (draggedSlot.ChestInventoryController != null && targetSlot.PlayerInventoryController != null))
                {
                    OnItemTransferBetweenInventories?.Invoke(draggedSlot, targetSlot);
                }
                // Przypadek: przeci�gamy w obr�bie tego samego ekwipunku (swap w tej samej li�cie)
                else if ((draggedSlot.PlayerInventoryController != null && targetSlot.PlayerInventoryController != null) ||
                         (draggedSlot.ChestInventoryController != null && targetSlot.ChestInventoryController != null))
                {
                    OnItemChangeSlotInTheSameList?.Invoke(draggedSlot, targetSlot);
                }
            }
        }
        else
        {
            // Upuszczamy na pusty obszar ekwipunku (ale nie na slot)
            OnItemDropToInventory?.Invoke(this, eventData);
        }

        // Usu� wizualn� reprezentacj� przeci�ganego obiektu
        if (m_DraggingIcon != null)
        {
            Destroy(m_DraggingIcon);
        }
    }

    /// <summary>
    /// Wyszukuje komponent typu T w rodzicach danego obiektu.
    /// </summary>
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

    public void SetItemIcon(Sprite icon)
    {
        if (TargetSlot == null)
            TargetSlot = GetComponent<Image>();
        TargetSlot.sprite = icon;
    }
    public void SetItem(InventoryItemDatabes.Item item)
    {
        this._item = item;

        if (item != null && item.itemIcon != null)
        {
            ItemIcon = item.itemIcon;
        }
        else
        {
            ItemIcon = null;

            Transform itemIconTransform = transform.Find("ItemIcon");
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
}