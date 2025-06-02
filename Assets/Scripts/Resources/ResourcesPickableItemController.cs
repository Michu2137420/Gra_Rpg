using System;
using UnityEngine;
using static InventoryItemDatabes.Item;

public class ResourcesPickableItemController : MonoBehaviour
{
    public InventoryItemDatabes.Item pickAbleItem;

    // Definicja eventu
    public static event Action OnItemPickedUp;

    private void Start()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = pickAbleItem.itemIcon;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                for (int i = 0; i < playerController.PlayerItems.Count; i++)
                {
                    if (playerController.PlayerItems[i] == null)
                    {
                        playerController.AddItemToInventory(pickAbleItem);
                        OnItemPickedUp?.Invoke();
                        break;
                    }
                }

                Debug.Log("Picked up: " + pickAbleItem.itemName);
                Destroy(gameObject); // Zniszcz obiekt po podniesieniu
            }
        }
    }
}
