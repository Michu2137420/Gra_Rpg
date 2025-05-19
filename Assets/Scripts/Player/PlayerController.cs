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
        PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerItems);
    }

    void Awake()
    {
        // Inicjalizacja listy o wielko�ci r�wnej liczbie slot�w w Inv, najpierw pierwotne itemy, reszta null
        if (PlayerInventoryController != null)
        {
            PlayerItems = new List<InventoryItemDatabes.Item>(PlayerInventoryController.numberOfSlots);
            int slots = PlayerInventoryController.numberOfSlots;
            int initialCount = PlayerInitialInventoryItems != null ? PlayerInitialInventoryItems.Count : 0;

            // Dodaj pierwotne itemy
            for (int i = 0; i < slots; i++)
            {
                if (PlayerInitialInventoryItems != null && i < PlayerInitialInventoryItems.Count)
                {
                    PlayerItems.Add(PlayerInitialInventoryItems[i]);
                }
                else
                {
                    PlayerItems.Add(null);
                }
            }
        }
        else
        {
            PlayerItems = new List<InventoryItemDatabes.Item>();
        }
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
                foreach (var slot in FindObjectsByType<Slot>(FindObjectsSortMode.None))
                {
                    slot.SetCurrentPlayerItems(PlayerItems);
                }
            }
        }
    }
    // tak naprawde robie t� funkcje czy dzia�a logika podnoszenia item�w i ich dzia�ania uzywanie item�w bedzei w ekwipunku
    public void FunctionToManageHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Current Health: " + currentHealth);
    }




}
