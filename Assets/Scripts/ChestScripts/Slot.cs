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

    //event do obs³ugi wyboru slotu na pasku skrótów obs³ugiwany w PlayerController 
    public static class HotbarEvents
    {
        public static Action<int> OnSlotSelected;
    }

    /// <summary>
    /// Inicjalizuje referencje do slotu, kontrolerów ekwipunku oraz ustawia kolor pocz¹tkowy.
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
    /// Obs³uguje wybór slotu na pasku skrótów po naciœniêciu odpowiedniego klawisza.
    /// </summary>
    void Update()
    {
        //logika do u¿ycia przedmiotu z paska skrótów
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
    /// Subskrybuje event wyboru slotu na pasku skrótów.
    /// </summary>
    void OnEnable()
    {
        // Subskrybuj event
        HotbarEvents.OnSlotSelected += OnOtherSlotSelected;
    }

    /// <summary>
    /// Odsubskrybowuje event wyboru slotu na pasku skrótów.
    /// </summary>
    private void OnDisable()
    {
        // Odsubskrybuj event
        HotbarEvents.OnSlotSelected -= OnOtherSlotSelected;
    }

    /// <summary>
    /// Ukrywa ramkê zaznaczenia, jeœli wybrano inny slot na pasku skrótów.
    /// </summary>
    private void OnOtherSlotSelected(int selectedSlotID)
    {
        if (selectedSlotID != SlotID && _itemFrame != null)
        {
            _itemFrame.SetActive(false);
        }
    }

    /// <summary>
    /// Obs³uguje logikê wyboru slotu na pasku skrótów (zaznaczenie, powiadomienie innych slotów).
    /// </summary>
    private void OnUseBarSlotSelected()
    {
        if (_item != null)
        {
            Debug.Log($"Wybrano slot {SlotID + 1} na pasku skrótów: {_item.itemName}");

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
            Debug.Log($"Slot {SlotID + 1} na pasku skrótów jest pusty.");
            HotbarEvents.OnSlotSelected?.Invoke(-1);
        }
    }

    /// <summary>
    /// Wywo³ywana po klikniêciu na slot - wyœwietla informacje o przedmiocie.
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
    /// Zmienia kolor slotu po najechaniu kursorem oraz ustawia indeksy slotów do zamiany.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {

        // Ustaw slotSecond na ten slot, bo na niego naje¿d¿amy
        _slotSecond = SlotID;

        // Jeœli przeci¹gamy inny slot, ustaw slotFirst na slot przeci¹gany
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
    /// Zwraca aktualn¹ listê przedmiotów gracza bezpoœrednio z kontrolera
    /// </summary>
    public List<InventoryItemDatabes.Item> GetCurrentPlayerItems()
    {
        if (PlayerInventoryController != null)
        {
            return PlayerInventoryController.currentyDisplayedItems;
        }

        // Fallback - spróbuj znaleŸæ PlayerController w scenie
        var playerController = FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            return playerController.PlayerItems;
        }

        Debug.LogWarning("Nie mo¿na znaleŸæ PlayerInventoryController lub PlayerController!");
        return null;
    }

    /// <summary>
    /// Zwraca aktualn¹ listê przedmiotów skrzynki bezpoœrednio z kontrolera
    /// </summary>
    public List<InventoryItemDatabes.Item> GetCurrentChestItems()
    {
        if (ChestInventoryController != null)
        {
            return ChestInventoryController.currentyDisplayedItems;
        }

        Debug.LogWarning("Nie mo¿na znaleŸæ ChestInventoryController!");
        return null;
    }

    /// <summary>
    /// Rozpoczyna przeci¹ganie przedmiotu - tworzy wizualn¹ reprezentacjê przeci¹ganego obiektu.
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
        // Ustaw rozmiar przeci¹ganego obrazka na 50x50 pikseli
        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        image.raycastTarget = false;

        m_DraggingPlane = dragOnSurfaces ? transform as RectTransform : canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    /// <summary>
    /// Aktualizuje pozycjê przeci¹ganego obiektu podczas przeci¹gania.
    /// </summary>
    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }

    /// <summary>
    /// Ustawia pozycjê przeci¹ganego obiektu na podstawie pozycji kursora.
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
    /// Koñczy przeci¹ganie przedmiotu - wywo³uje odpowiednie eventy dla ChestTransferingItemsLogic.
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
            // Okreœl Ÿród³owy slot (dragged) i docelowy (target)
            Slot draggedSlot = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<Slot>() : null;
            if (draggedSlot != null && draggedSlot != targetSlot)
            {
                // Przypadek: przeci¹gamy miêdzy ró¿nymi ekwipunkami
                if ((draggedSlot.PlayerInventoryController != null && targetSlot.ChestInventoryController != null) ||
                    (draggedSlot.ChestInventoryController != null && targetSlot.PlayerInventoryController != null))
                {
                    OnItemTransferBetweenInventories?.Invoke(draggedSlot, targetSlot);
                }
                // Przypadek: przeci¹gamy w obrêbie tego samego ekwipunku (swap w tej samej liœcie)
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

        // Usuñ wizualn¹ reprezentacjê przeci¹ganego obiektu
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