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
        // Inicjalizacja listy o wielko�ci r�wnej liczbie slot�w w Inv
        if (PlayerInventoryController != null)
        {
            int slots = PlayerInventoryController.numberOfSlots;

            // Najpierw stw�rz pust� list� wype�nion� nullami
            PlayerItems = new List<InventoryItemDatabes.Item>(slots);
            for (int i = 0; i < slots; i++)
            {
                PlayerItems.Add(null);
            }

            // Teraz dodaj pocz�tkowe przedmioty u�ywaj�c AddItemToInventory
            if (PlayerInitialInventoryItems != null)
            {
                foreach (var initialItem in PlayerInitialInventoryItems)
                {
                    if (initialItem != null)
                    {
                        // U�ywamy AddItemToInventory aby zachowa� logik� stackowania
                        bool success = AddItemToInventory(initialItem);

                        if (!success)
                        {
                            Debug.LogWarning($"Nie uda�o si� doda� pocz�tkowego przedmiotu: {initialItem.itemName}. Inwentarz mo�e by� pe�ny.");
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

        // Je�li item jest stackowalny, spr�buj znale�� ju� istniej�cy taki sam i zwi�ksz ilo��
        if (newItem.isStackable)
        {
            for (int i = 0; i < PlayerItems.Count; i++)
            {
                var existingItem = PlayerItems[i];
                if (existingItem != null &&
                    existingItem.itemID == newItem.itemID &&
                    existingItem.isStackable)
                {
                    // Zwi�ksz ilo�� istniej�cego itemu
                    existingItem.itemAmount += newItem.itemAmount;

                    // Od�wie� wy�wietlanie inwentarza je�li jest otwarty
                    if (PlayerInventoryController.isOpen)
                    {
                        PlayerInventoryController.DisplayItemsInInventory(PlayerItems);
                    }

                    // Od�wie� pasek u�ycia
                    PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerItems);

                    return true;
                }
            }
        }

        // Je�li nie stackowalny lub nie znaleziono stackowalnego, dodaj do pierwszego wolnego slotu
        for (int i = 0; i < PlayerItems.Count; i++)
        {
            if (PlayerItems[i] == null)
            {
                // Stw�rz now� kopi� itemu aby unikn�� problem�w z referencjami
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

                // Od�wie� wy�wietlanie inwentarza je�li jest otwarty
                if (PlayerInventoryController.isOpen)
                {
                    PlayerInventoryController.DisplayItemsInInventory(PlayerItems);
                }

                // Od�wie� pasek u�ycia
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
        // Logika poruszania si� postaci
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

            // PlayerUseBarInventory jest widoczny tylko gdy ekwipunek jest zamkni�ty
            if (PlayerUseBarController.PlayerUseBarInventory != null)
            {
                PlayerUseBarController.PlayerUseBarInventory.SetActive(!nowOpen);

                // Je�li ekwipunek jest zamykany, od�wie�amy pasek u�ycia
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
    // tak naprawde robie t� funkcje czy dzia�a logika podnoszenia item�w i ich dzia�ania uzywanie item�w bedzei w ekwipunku
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
