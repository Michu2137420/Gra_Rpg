using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemDatabes : MonoBehaviour
{
    //Lista zawieraj¹ca itemy w skrzynce

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
    }

    [System.Serializable]
    public class Item
    {
        public enum ItemType //Typ itemu mozna dodaæ potem wiecej
        {
            Weapon,
            Armor,
            Consumable,
            Potion,
        }
        //Wszystkie zmienne itemu
        public string itemName;
        public string itemDescription;
        public int itemID;
        public int itemValue;
        public int itemAmount;
        public ItemType itemType;
        public Sprite itemIcon;
        public bool isStackable = false;
        //Konstruktor itemu
        public Item(string name, string description, int id, int value, int amount, Sprite icon, ItemType itemType)
        {
            itemName = name;
            itemDescription = description;
            itemID = id;
            itemValue = value;
            itemAmount = amount;
            itemIcon = icon;
            isStackable=false;
            this.itemType = itemType;
        }
    }




}
