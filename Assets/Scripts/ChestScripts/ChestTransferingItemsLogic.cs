using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestTransferingItemsLogic : MonoBehaviour
{
    public static ChestTransferingItemsLogic Instance;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Slot.OnItemChangeSlotInTheSameList += HandleSwapItemsInSameList;
        Slot.OnItemTransferBetweenInventories += HandleTransferBetweenInventories;
        Slot.OnItemDropToInventory += HandleDropToInventory;
    }

    private void OnDisable()
    {
        Slot.OnItemChangeSlotInTheSameList -= HandleSwapItemsInSameList;
        Slot.OnItemTransferBetweenInventories -= HandleTransferBetweenInventories;
        Slot.OnItemDropToInventory -= HandleDropToInventory;
    }

    /// <summary>
    /// Obs³uguje zamianê itemów w tym samym ekwipunku poprzez event
    /// </summary>
    private void HandleSwapItemsInSameList(Slot draggedSlot, Slot targetSlot)
    {
        Debug.Log($"[HandleSwapItemsInSameList] draggedSlot: {draggedSlot.SlotID}, targetSlot: {targetSlot.SlotID}");

        if (draggedSlot.PlayerInventoryController != null && targetSlot.PlayerInventoryController != null)
        {
            var playerItems = draggedSlot.GetCurrentPlayerItems();
            if (playerItems != null)
            {
                SwapItemsInSameList(draggedSlot.SlotID, targetSlot.SlotID, playerItems, draggedSlot.PlayerInventoryController);
            }
        }
        else if (draggedSlot.ChestInventoryController != null && targetSlot.ChestInventoryController != null)
        {
            var chestItems = draggedSlot.GetCurrentChestItems();
            if (chestItems != null)
            {
                SwapItemsInSameList(draggedSlot.SlotID, targetSlot.SlotID, chestItems, draggedSlot.ChestInventoryController);
            }
        }
    }

    /// <summary>
    /// Obs³uguje transfer itemów miêdzy ró¿nymi ekwipunkami poprzez event
    /// </summary>
    private void HandleTransferBetweenInventories(Slot draggedSlot, Slot targetSlot)
    {
        Debug.Log($"[HandleTransferBetweenInventories] draggedSlot: {draggedSlot.SlotID}, targetSlot: {targetSlot.SlotID}");
        TryMoveOrSwapBetweenInventories(draggedSlot, targetSlot);
    }

    /// <summary>
    /// Obs³uguje upuszczenie itemu na ekwipunek (ale nie na konkretny slot)
    /// </summary>
    private void HandleDropToInventory(Slot draggedSlot, PointerEventData eventData)
    {
        Debug.Log($"[HandleDropToInventory] draggedSlot: {draggedSlot.SlotID}, eventData: {eventData.pointerEnter.name}");
        HandleDropToEmptyInventorySpace(draggedSlot, eventData);
    }

    /// <summary>
    /// Zamienia miejscami dwa przedmioty w tej samej liœcie (np. w ekwipunku gracza lub skrzynki).
    /// </summary>
    public void SwapItemsInSameList(int firstIndex, int secondIndex, List<InventoryItemDatabes.Item> itemList, MonoBehaviour controller)
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

        // Aktualizuj widok
        if (controller is ChestInventoryController chestCtrl)
        {
            chestCtrl.DisplayItemsInInventory(itemList);
        }
        else if (controller is PlayerInventoryController playerCtrl)
        {
            playerCtrl.DisplayItemsInInventory(itemList);
        }
    }

    /// <summary>
    /// Obs³uguje upuszczenie itemu na pusty obszar ekwipunku
    /// </summary>
    private void HandleDropToEmptyInventorySpace(Slot draggedSlot, PointerEventData eventData)
    {
        var chestCtrl = Slot.FindInParents<ChestInventoryController>(eventData.pointerEnter);
        var playerCtrl = Slot.FindInParents<PlayerInventoryController>(eventData.pointerEnter);

        // Przeci¹gamy z gracza do skrzynki
        if (draggedSlot.PlayerInventoryController != null && chestCtrl != null)
        {
            var playerItems = draggedSlot.GetCurrentPlayerItems();
            var chestItems = chestCtrl.currentyDisplayedItems;

            if (playerItems != null && chestItems != null)
            {
                int freeIndex = chestItems.FindIndex(i => i == null);
                if (freeIndex != -1 && playerItems[draggedSlot.SlotID] != null)
                {
                    chestItems[freeIndex] = playerItems[draggedSlot.SlotID];
                    playerItems[draggedSlot.SlotID] = null;
                    chestCtrl.DisplayItemsInInventory(chestItems);
                    draggedSlot.PlayerInventoryController.DisplayItemsInInventory(playerItems);
                }
            }
        }
        // Przeci¹gamy ze skrzynki do gracza
        else if (draggedSlot.ChestInventoryController != null && playerCtrl != null)
        {
            var chestItems = draggedSlot.GetCurrentChestItems();
            var playerItems = playerCtrl.currentyDisplayedItems;

            if (chestItems != null && playerItems != null)
            {
                int freeIndex = playerItems.FindIndex(i => i == null);
                if (freeIndex != -1 && chestItems[draggedSlot.SlotID] != null)
                {
                    playerItems[freeIndex] = chestItems[draggedSlot.SlotID];
                    chestItems[draggedSlot.SlotID] = null;
                    playerCtrl.DisplayItemsInInventory(playerItems);
                    draggedSlot.ChestInventoryController.DisplayItemsInInventory(chestItems);
                }
            }
        }
    }

    /// <summary>
    /// Przenosi lub zamienia przedmioty pomiêdzy dwoma listami (np. gracza i skrzynki).
    /// </summary>
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

    /// <summary>
    /// Próbuje przenieœæ lub zamieniæ przedmioty pomiêdzy ekwipunkiem gracza a skrzynk¹.
    /// </summary>
    public void TryMoveOrSwapBetweenInventories(Slot draggedSlot, Slot targetSlot)
    {
        bool draggedIsChest = draggedSlot.ChestInventoryController != null;
        bool targetIsChest = targetSlot.ChestInventoryController != null;
        bool draggedIsPlayer = draggedSlot.PlayerInventoryController != null;
        bool targetIsPlayer = targetSlot.PlayerInventoryController != null;

        // Wymiana tylko jeœli sloty s¹ z ró¿nych ekwipunków
        if ((draggedIsChest && targetIsPlayer) || (draggedIsPlayer && targetIsChest))
        {
            List<InventoryItemDatabes.Item> fromList, toList;
            int fromIndex, toIndex;
            MonoBehaviour fromCtrl, toCtrl;

            // Zawsze "draggedSlot" to Ÿród³o, "targetSlot" to cel
            if (draggedIsPlayer && targetIsChest)
            {
                fromList = draggedSlot.GetCurrentPlayerItems();
                toList = targetSlot.GetCurrentChestItems();
                fromIndex = draggedSlot.SlotID;
                toIndex = targetSlot.SlotID;
                fromCtrl = draggedSlot.PlayerInventoryController;
                toCtrl = targetSlot.ChestInventoryController;
            }
            else if (draggedIsChest && targetIsPlayer)
            {
                fromList = draggedSlot.GetCurrentChestItems();
                toList = targetSlot.GetCurrentPlayerItems();
                fromIndex = draggedSlot.SlotID;
                toIndex = targetSlot.SlotID;
                fromCtrl = draggedSlot.ChestInventoryController;
                toCtrl = targetSlot.PlayerInventoryController;
            }
            else
            {
                // Nie powinno siê zdarzyæ
                Debug.LogWarning("Nieprawid³owa kombinacja slotów!");
                return;
            }

            // SprawdŸ czy listy nie s¹ null
            if (fromList != null && toList != null)
            {
                MoveOrSwapBetweenLists(fromIndex, fromList, toIndex, toList, fromCtrl, toCtrl);
            }
            else
            {
                Debug.LogWarning("Jedna z list jest null - nie mo¿na wykonaæ transferu!");
            }
        }
    }

}