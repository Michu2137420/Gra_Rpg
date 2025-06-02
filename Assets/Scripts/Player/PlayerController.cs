using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]PlayerInventoryController PlayerInventoryController;
    [SerializeField] PlayerUseBarController PlayerUseBarController;
    public List<InventoryItemDatabes.Item> PlayerItems;
    public List<InventoryItemDatabes.Item> PlayerInitialInventoryItems;
    [SerializeField] float speed;
    public int maxHealth;
    public int currentHealth;



    // Start is called before the first frame update
    void Start()    
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerItems);
    }

    void Awake()
    {
       InitializeItemsInList();
    }

    void Update()
    {
        // Logika poruszania siê postaci
        movementLogic();

    }
    private void movementLogic()
    {
        Vector2 move = transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            move.y += 0.1f * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move.y -= 0.1f * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move.x -= 0.1f * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move.x += 0.1f * speed;
        }
        transform.position = move;
    }

   

    // tak naprawde robie t¹ funkcje czy dzia³a logika podnoszenia itemów i ich dzia³ania uzywanie itemów bedzei w ekwipunku
    public void FunctionToManageHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Current Health: " + currentHealth);
    }
    private void OnEnable()
    {
        ResourcesPickableItemController.OnItemPickedUp += HandleItemPickedUp;
    }

    private void OnDisable()
    {
        ResourcesPickableItemController.OnItemPickedUp -= HandleItemPickedUp;
    }

    private void HandleItemPickedUp()
    {
       PlayerInventoryController.DisplayItemsInInventory(PlayerItems);
        PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerItems);
    }

    private void InitializeItemsInList()
    {
        // Inicjalizacja listy o wielkoœci równej liczbie slotów w Inv
        if (PlayerInventoryController != null)
        {
            int slots = PlayerInventoryController.numberOfSlots;

            // Najpierw stwórz pust¹ listê wype³nion¹ nullami
            PlayerItems = new List<InventoryItemDatabes.Item>(slots);
            for (int i = 0; i < slots; i++)
            {
                PlayerItems.Add(null);
            }

            // Teraz dodaj pocz¹tkowe przedmioty u¿ywaj¹c AddItemToInventory
            if (PlayerInitialInventoryItems != null)
            {
                foreach (var initialItem in PlayerInitialInventoryItems)
                {
                    if (initialItem != null)
                    {
                        // U¿ywamy AddItemToInventory aby zachowaæ logikê stackowania
                        bool success = AddItemToInventory(initialItem);

                        if (!success)
                        {
                            Debug.LogWarning($"Nie uda³o siê dodaæ pocz¹tkowego przedmiotu: {initialItem.itemName}. Inwentarz mo¿e byæ pe³ny.");
                        }
                    }
                }
            }
        }
        else
        {
            PlayerItems = new List<InventoryItemDatabes.Item>();
            Debug.LogWarning("PlayerInventoryController is null!");
        }
    }
    public bool AddItemToInventory(InventoryItemDatabes.Item newItem)
    {
        if (newItem == null)
            return false;

        // Jeœli item jest stackowalny, spróbuj znaleŸæ ju¿ istniej¹cy taki sam i zwiêksz iloœæ
        if (newItem.isStackable)
        {
            for (int i = 0; i < PlayerItems.Count; i++)
            {
                var existingItem = PlayerItems[i];
                if (existingItem != null &&
                    existingItem.itemID == newItem.itemID &&
                    existingItem.isStackable)
                {
                    // Zwiêksz iloœæ istniej¹cego itemu
                    existingItem.itemAmount += newItem.itemAmount;

                    // Odœwie¿ wyœwietlanie inwentarza jeœli jest otwarty
                    if (PlayerInventoryController.isOpen)
                    {
                        PlayerInventoryController.DisplayItemsInInventory(PlayerItems);
                    }

                    // Odœwie¿ pasek u¿ycia
                    PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerItems);

                    return true;
                }
            }
        }

        // Jeœli nie stackowalny lub nie znaleziono stackowalnego, dodaj do pierwszego wolnego slotu
        for (int i = 0; i < PlayerItems.Count; i++)
        {
            if (PlayerItems[i] == null)
            {
                // Utwórz now¹ instancjê ScriptableObject i skopiuj dane z newItem
                var itemInstance = ScriptableObject.CreateInstance<InventoryItemDatabes.Item>();
                itemInstance.itemName = newItem.itemName;
                itemInstance.itemDescription = newItem.itemDescription;
                itemInstance.itemID = newItem.itemID;
                itemInstance.itemValue = newItem.itemValue;
                itemInstance.itemAmount = newItem.itemAmount;
                itemInstance.itemIcon = newItem.itemIcon;
                itemInstance.itemType = newItem.itemType;
                itemInstance.isStackable = newItem.isStackable;

                PlayerItems[i] = itemInstance;

                // Odœwie¿ wyœwietlanie inwentarza jeœli jest otwarty
                if (PlayerInventoryController.isOpen)
                {
                    PlayerInventoryController.DisplayItemsInInventory(PlayerItems);
                }

                // Odœwie¿ pasek u¿ycia
                PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerItems);

                return true;
            }

        }

        // Brak miejsca w inwentarzu
        Debug.Log("Brak miejsca w inwentarzu!");
        return false;
    }


}
