using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Rendering;

//Uwaga Wazne dziala juz zmiana slotow i aktualizowanie tego jednakze nie dziala jeszcze upladowanie tej nowej listy do manadzera ekwipunku
[RequireComponent(typeof(Image))]
public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    Image TargetSlot;
    GameObject slot;
    public ChestInventoryController chestInventoryController;
    public PlayerInventoryController playerInventoryController;
    public Color32 normalcolor;
    public Color32 enterColor;
    public Sprite itemIcon;
    public int slotID;
    private InventoryItemDatabes.Item item; 
    private List<InventoryItemDatabes.Item> currentlyListOfSwapingItemsInChest;
    private List<InventoryItemDatabes.Item> currentlyListOfSwapingItemsInPlayer;
    public bool dragOnSurfaces = true;

    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    private static int slotFirst;
    private static int slotSecond;

    void Start()
    {
        slot = gameObject;
        TargetSlot = GetComponent<Image>();
        TargetSlot.color = normalcolor;
        chestInventoryController = FindInParents<ChestInventoryController>(gameObject);
        playerInventoryController = FindInParents<PlayerInventoryController>(gameObject);

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null)
        {
            //Debug.Log("Clicked on item: " + item.itemIcon);
            InfoItemSlot.instance.DisplayItemIconWhenClicked(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TargetSlot.color = normalcolor;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        TargetSlot.color = enterColor;
        //Debug.Log("Pointer Entered Slot: " + slotID);

        // Ustaw slotSecond na ten slot, bo na niego naje¿d¿amy
        slotSecond = slotID;

        // Jeœli przeci¹gamy inny slot, ustaw slotFirst na slot przeci¹gany
        if (eventData.pointerDrag != null && eventData.pointerDrag != gameObject)
        {
            Slot draggedSlot = eventData.pointerDrag.GetComponent<Slot>();
            if (draggedSlot != null)
            {
                slotFirst = draggedSlot.slotID;
                //Debug.Log("Dragged slotID (slotFirst): " + slotFirst + ", Entered slotID (slotSecond): " + slotSecond);
            }
            else
            {
                slotFirst = slotID;
                //Debug.Log("Dragged slot null, slotFirst i slotSecond: " + slotFirst);
            }
        }
        else
        {
            slotFirst = slotID;
            //Debug.Log("Brak przeci¹gania, slotFirst i slotSecond: " + slotFirst);
        }
    }
    public void SetItemIcon(Sprite icon)
    {
        if (TargetSlot == null)
            TargetSlot = GetComponent<Image>();
        TargetSlot.sprite = icon;
    }
    public void SetCurrentChestItems(List<InventoryItemDatabes.Item> items)
    {
        currentlyListOfSwapingItemsInChest = items;
    }
    public void SetCurrentPlayerItems(List<InventoryItemDatabes.Item> items)
    {
        currentlyListOfSwapingItemsInPlayer = items;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null || item.itemIcon == null)
            return;

        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;
        m_DraggingIcon = new GameObject("DraggedIcon");
        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();
        image.sprite = item.itemIcon;
        // Ustaw rozmiar przeci¹ganego obrazka na 50x50 pikseli
        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        image.raycastTarget = false;

        m_DraggingPlane = dragOnSurfaces ? transform as RectTransform : canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }

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
                // Przypadek: przeci¹gamy z gracza do skrzynki
                if (draggedSlot.playerInventoryController != null && targetSlot.chestInventoryController != null)
                {
                    draggedSlot.TryMoveOrSwapBetweenInventories(targetSlot);
                }
                // Przypadek: przeci¹gamy ze skrzynki do gracza
                else if (draggedSlot.chestInventoryController != null && targetSlot.playerInventoryController != null)
                {
                    draggedSlot.TryMoveOrSwapBetweenInventories(targetSlot);
                }
                // Przypadek: przeci¹gamy w obrêbie tego samego ekwipunku (swap w tej samej liœcie)
                else if (draggedSlot.playerInventoryController != null && targetSlot.playerInventoryController != null)
                {
                    SwapItemsInSameList(draggedSlot.slotID, targetSlot.slotID, draggedSlot.currentlyListOfSwapingItemsInPlayer);
                }
                else if (draggedSlot.chestInventoryController != null && targetSlot.chestInventoryController != null)
                {
                    SwapItemsInSameList(draggedSlot.slotID, targetSlot.slotID, draggedSlot.currentlyListOfSwapingItemsInChest);
                }
            }
        }
        else
        {
            // Nowa logika: sprawdŸ, czy kursor jest nad ekwipunkiem (ale nie nad slotem)
            var chestCtrl = FindInParents<ChestInventoryController>(eventData.pointerEnter);
            var playerCtrl = FindInParents<PlayerInventoryController>(eventData.pointerEnter);

            // Przeci¹gamy z gracza do skrzynki
            if (playerInventoryController != null && chestCtrl != null)
            {
                int freeIndex = chestCtrl.currentyDisplayedItems.FindIndex(i => i == null);
                if (freeIndex != -1 && currentlyListOfSwapingItemsInPlayer[slotID] != null)
                {
                    chestCtrl.currentyDisplayedItems[freeIndex] = currentlyListOfSwapingItemsInPlayer[slotID];
                    currentlyListOfSwapingItemsInPlayer[slotID] = null;
                    chestCtrl.DisplayItemsInInventory(chestCtrl.currentyDisplayedItems);
                    playerInventoryController.DisplayItemsInInventory(currentlyListOfSwapingItemsInPlayer);
                }
            }
            // Przeci¹gamy ze skrzynki do gracza
            else if (chestInventoryController != null && playerCtrl != null)
            {
                int freeIndex = playerCtrl.currentyDisplayedItems.FindIndex(i => i == null);
                if (freeIndex != -1 && currentlyListOfSwapingItemsInChest[slotID] != null)
                {
                    playerCtrl.currentyDisplayedItems[freeIndex] = currentlyListOfSwapingItemsInChest[slotID];
                    currentlyListOfSwapingItemsInChest[slotID] = null;
                    playerCtrl.DisplayItemsInInventory(playerCtrl.currentyDisplayedItems);
                    chestInventoryController.DisplayItemsInInventory(currentlyListOfSwapingItemsInChest);
                }
            }
        }
        // Debug.Log("Witam");
        if (m_DraggingIcon != null)
        {
            Destroy(m_DraggingIcon);
        }
        UpdateInventoryView();

    }
    public void SwapItemsInSameList(int firstIndex, int secondIndex, List<InventoryItemDatabes.Item> itemList)
    {
        if (itemList == null || firstIndex < 0 || secondIndex < 0 ||
            firstIndex >= itemList.Count || secondIndex >= itemList.Count)
        {
            Debug.LogWarning("B³êdne indeksy lub lista!");
            return;
        }
        var temp = itemList[firstIndex];
        itemList[firstIndex] = itemList[secondIndex];
        itemList[secondIndex] = temp;
        UpdateInventoryView();
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

    public void SetItem(InventoryItemDatabes.Item item)
    {
        this.item = item;
        // Nie ustawiaj ikony na slocie, tylko przechowuj sprite w itemIcon
        if (item != null && item.itemIcon != null)
            itemIcon = item.itemIcon;
        else
            itemIcon = null;
        SetItemIcon(null);
    }

 

    public void MoveOrSwapBetweenLists(int fromIndex, List<InventoryItemDatabes.Item> fromList, int toIndex, List<InventoryItemDatabes.Item> toList, MonoBehaviour fromController, MonoBehaviour toController)
    {
        // Dodaj debugowanie, aby sprawdziæ, która lista jest Ÿród³owa, a która docelowa
        string fromType = fromController is ChestInventoryController ? "CHEST" : "PLAYER";
        string toType = toController is ChestInventoryController ? "CHEST" : "PLAYER";
        Debug.Log($"[MoveOrSwapBetweenLists] FROM: {fromType} (index {fromIndex}) -> TO: {toType} (index {toIndex})");

        if (fromList == null || toList == null ||
            fromIndex < 0 || fromIndex >= fromList.Count ||
            toIndex < 0 || toIndex >= toList.Count)
        {
            Debug.LogWarning("B³êdne indeksy lub listy!");
            return;
        }

        var fromItem = fromList[fromIndex];
        var toItem = toList[toIndex];

        // Jeœli nie ma co przenosiæ, wyjdŸ
        if (fromItem == null && toItem == null)
            return;

        // Przenoszenie do pustego slotu
        if (fromItem != null && toItem == null)
        {
            toList[toIndex] = fromItem;
            fromList[fromIndex] = null;
            Debug.Log($"Przeniesiono item z {fromType}[{fromIndex}] do {toType}[{toIndex}]");
        }
        // Przenoszenie z pustego slotu (nie powinno siê zdarzyæ, ale na wszelki wypadek)
        else if (fromItem == null && toItem != null)
        {
            fromList[fromIndex] = toItem;
            toList[toIndex] = null;
            Debug.Log($"Przeniesiono item z {toType}[{toIndex}] do {fromType}[{fromIndex}]");
        }
        // Zamiana miejscami
        else
        {
            fromList[fromIndex] = toItem;
            toList[toIndex] = fromItem;
            Debug.Log($"Zamieniono itemy pomiêdzy {fromType}[{fromIndex}] a {toType}[{toIndex}]");
        }

        // Aktualizuj widoki
        if (fromController is ChestInventoryController chestCtrl1)
            chestCtrl1.DisplayItemsInInventory(fromList);
        else if (fromController is PlayerInventoryController playerCtrl1)
            playerCtrl1.DisplayItemsInInventory(fromList);

        if (toController is ChestInventoryController chestCtrl2)
            chestCtrl2.DisplayItemsInInventory(toList);
        else if (toController is PlayerInventoryController playerCtrl2)
            playerCtrl2.DisplayItemsInInventory(toList);

        Debug.Log($"[MoveOrSwapBetweenLists] Zaktualizowano widoki: {fromType} oraz {toType}");
    }
    public void TryMoveOrSwapBetweenInventories(Slot targetSlot)
    {
        bool thisIsChest = chestInventoryController != null;
        bool targetIsChest = targetSlot.chestInventoryController != null;
        bool thisIsPlayer = playerInventoryController != null;
        bool targetIsPlayer = targetSlot.playerInventoryController != null;

        // Wymiana tylko jeœli sloty s¹ z ró¿nych ekwipunków
        if ((thisIsChest && targetIsPlayer) || (thisIsPlayer && targetIsChest))
        {
            List<InventoryItemDatabes.Item> fromList, toList;
            int fromIndex, toIndex;
            MonoBehaviour fromCtrl, toCtrl;

            // Poprawka: zawsze "this" to Ÿród³o, "targetSlot" to cel
            if (thisIsPlayer && targetIsChest)
            {
                fromList = currentlyListOfSwapingItemsInPlayer;
                toList = targetSlot.currentlyListOfSwapingItemsInChest;
                fromIndex = slotID;
                toIndex = targetSlot.slotID;
                fromCtrl = playerInventoryController;
                toCtrl = targetSlot.chestInventoryController;
            }
            else if (thisIsChest && targetIsPlayer)
            {
                fromList = currentlyListOfSwapingItemsInChest;
                toList = targetSlot.currentlyListOfSwapingItemsInPlayer;
                fromIndex = slotID;
                toIndex = targetSlot.slotID;
                fromCtrl = chestInventoryController;
                toCtrl = targetSlot.playerInventoryController;
            }
            else
            {
                // Nie powinno siê zdarzyæ
                Debug.LogWarning("Nieprawid³owa kombinacja slotów!");
                return;
            }

            MoveOrSwapBetweenLists(fromIndex, fromList, toIndex, toList, fromCtrl, toCtrl);
        }
    }
    public void UpdateInventoryView()
    {
        if (chestInventoryController != null)
        {
            chestInventoryController.DisplayItemsInInventory(currentlyListOfSwapingItemsInChest);
        }
        else if (playerInventoryController != null)
        {
            playerInventoryController.DisplayItemsInInventory(currentlyListOfSwapingItemsInPlayer);
        }
        //Debug.Log("Zaktualizowano widok ekwipunku");
    }
}
