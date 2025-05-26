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
                // Stwórz now¹ kopiê itemu aby unikn¹æ problemów z referencjami
                PlayerItems[i] = new InventoryItemDatabes.Item(
                    newItem.itemName,
                    newItem.itemDescription,
                    newItem.itemID,
                    newItem.itemValue,
                    newItem.itemAmount,
                    newItem.itemIcon,
                    newItem.itemType
                );
                PlayerItems[i].isStackable = newItem.isStackable;

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
    void Update()
    {
        // Logika poruszania siê postaci
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            bool nowOpen = !PlayerInventoryController.isOpen;
            PlayerInventoryController.playerInventory.SetActive(nowOpen);
            PlayerInventoryController.isOpen = nowOpen;

            // PlayerUseBarInventory jest widoczny tylko gdy ekwipunek jest zamkniêty
            if (PlayerUseBarController.PlayerUseBarInventory != null)
            {
                PlayerUseBarController.PlayerUseBarInventory.SetActive(!nowOpen);

                // Jeœli ekwipunek jest zamykany, odœwie¿amy pasek u¿ycia
                if (!nowOpen)
                {
                    PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerItems);
                }
            }

            if (nowOpen)
            {
                PlayerInventoryController.DisplayItemsInInventory(PlayerItems);
            }
        }
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



}
