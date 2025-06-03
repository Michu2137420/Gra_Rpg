using System;
using UnityEngine;

public class InventoryGuiManager : MonoBehaviour
{
    [SerializeField] PlayerInventoryController PlayerInventoryController;
    [SerializeField] PlayerUseBarController PlayerUseBarController;
    [SerializeField] PlayerController PlayerController;
    BuildingStation buildingStation; // Obiekt stacji budowania, je�li jest u�ywany
    GameObject singleInventoryDisplay { get; set; } // Obiekt pojedynczego ekwipunku (np. NPC, terminal)
    GameObject dualInventoryDisplay { get; set; } // Obiekt podw�jnego ekwipunku (np. skrzynka, skrzynia)
    private bool singleInventoryDisplayObject = false; // Czy wy�wietla� pojedynczy obiekt inwentarza
    private bool dualInventoryDisplayObject = false; // Czy wy�wietla� podw�jny obiekt inwentarza
    public bool nowOpen { get; private set; } = false; // Czy ekwipunek jest otwarty
    public static int howManyOpen = 0;


    public static Action openSecondInventory;
    public static Action closeSecondInventory;
    void Start()
    {

    }

    // Fixed the problematic Debug.Log statement to use proper string interpolation syntax.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Wartsoci Single i Dual {singleInventoryDisplayObject} {dualInventoryDisplayObject} oraz HowmanyOpen {howManyOpen}");
            inventoryOpenOrCloseLogic();
        }
    }
    private void inventoryOpenOrCloseLogic()
    {
        // je�li jeste� przy obiekcie z podw�jnym ekwipunkiem (np. skrzynka)
        if (dualInventoryDisplayObject)
        {
            Debug.Log("Jeste� przy obiekcie z podw�jnym ekwipunkiem");
            if (!nowOpen && howManyOpen == 0)
            {
                // je�li nic nie by�o otwarte, otw�rz ekwipunek gracza
                openDualInventory();
                //Debug.Log("otwieram eq gracza " + howManyOpen);
            }
            else if (nowOpen && howManyOpen == 1)
            {
                // je�li ekwipunek gracza jest otwarty, otw�rz r�wnie� drugi (obiektu)
                openSecondInventory?.Invoke();
                howManyOpen = 2;
                //Debug.Log("otwieram eq obiektu " + howManyOpen);
            }
            else if (nowOpen && howManyOpen == 2)
            {
                Debug.Log(howManyOpen);
                // je�li oba ekwipunki s� otwarte, zamknij je
                closeBothInventorys();
                //Debug.Log("zamykam oba ekwipunki " + howManyOpen);
            }
            return;
        }

        // Je�li jeste� przy obiekcie z pojedynczym ekwipunkiem (np. NPC, terminal)
        if (singleInventoryDisplayObject)
        {
            if (howManyOpen == 0)
            { Debug.Log("Jeste� przy obiekcie z pojedynczym ekwipunkiem");
                // Przy pojedynczym obiekcie zawsze zamykamy wszystko
                closeBothInventorys();
                buildingStation.OpeningBuildingInventory();
                return;
            }
            if (howManyOpen == 1)
            {
                buildingStation.CloseBuildingInventory();
                return;
            }
        }

        // Je�li NIE jeste� przy �adnym obiekcie - zarz�dzaj tylko swoim ekwipunkiem
        if (!dualInventoryDisplayObject && !singleInventoryDisplayObject)
        {
            if (nowOpen)
            {
                // Je�li ekwipunek gracza jest otwarty, zamknij go
                closePlayerInventory();
                // Debug.Log("zamykam eq gracza");
            }
            else
            {
                // Je�li ekwipunek gracza jest zamkni�ty, otw�rz go
                openPlayerInventory();
                // Debug.Log("otwieram eq gracza");
            }
        }
    }

    private void openPlayerInventory()
    {
        howManyOpen = 1;
        nowOpen = true;
        PlayerInventoryController.playerInventory.SetActive(true);
        PlayerInventoryController.isOpen = true;
        PlayerInventoryController.DisplayItemsInInventory(PlayerController.PlayerItems);

        // PlayerUseBarInventory jest ukryty gdy ekwipunek jest otwarty
        if (PlayerUseBarController.PlayerUseBarInventory != null)
        {
            PlayerUseBarController.PlayerUseBarInventory.SetActive(false);
        }
    }

    private void openPlayerInventoryOnly()
    {
        // Otwiera tylko ekwipunek gracza bez zmiany howManyOpen
        nowOpen = true;
        PlayerInventoryController.playerInventory.SetActive(true);
        PlayerInventoryController.isOpen = true;
        PlayerInventoryController.DisplayItemsInInventory(PlayerController.PlayerItems);
    }

    private void closePlayerInventory()
    {
        howManyOpen = 0;
        nowOpen = false;
        PlayerInventoryController.playerInventory.SetActive(false);
        PlayerInventoryController.isOpen = false;

        // Poka� pasek u�ycia
        if (PlayerUseBarController.PlayerUseBarInventory != null)
        {
            PlayerUseBarController.PlayerUseBarInventory.SetActive(true);
            PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerController.PlayerItems);
        }
    }

    private void closeBothInventorys()
    {
        // Zamknij oba ekwipunki
        howManyOpen = 0;
        nowOpen = false;
        PlayerInventoryController.playerInventory.SetActive(false);
        PlayerInventoryController.isOpen = false;

        // Zamknij drugi ekwipunek je�li jest otwarty
        closeSecondInventory?.Invoke();

        // Poka� pasek u�ycia
        if (PlayerUseBarController.PlayerUseBarInventory != null)
        {
            PlayerUseBarController.PlayerUseBarInventory.SetActive(true);
            PlayerUseBarController.DisplayItemsInUseBarInventory(PlayerController.PlayerItems);
        }
    }

    private void openDualInventory()
    {
        howManyOpen = 2;
        nowOpen = true;
        PlayerInventoryController.playerInventory.SetActive(true);
        PlayerInventoryController.isOpen = true;
        PlayerInventoryController.DisplayItemsInInventory(PlayerController.PlayerItems);

        openSecondInventory?.Invoke(); // Otw�rz drugi ekwipunek (np. skrzynka)

        // Ukryj pasek u�ycia
        if (PlayerUseBarController.PlayerUseBarInventory != null)
        {
            PlayerUseBarController.PlayerUseBarInventory.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var buildingStationComponent = collision.gameObject.GetComponent<BuildingStation>();
        if (collision.gameObject.CompareTag("SingleInventoryDisplay"))
        {
            if (buildingStationComponent != null)
            {
                singleInventoryDisplay = collision.gameObject;
                buildingStation = buildingStationComponent;
                Debug.Log("Collision with BuildingStation detected");
                singleInventoryDisplayObject = true;
                dualInventoryDisplayObject = false;
                return;
            }
            Debug.Log("Collision with SingleInventoryDisplay detected");
            singleInventoryDisplayObject = true;
            dualInventoryDisplayObject = false;
        }
        else if (collision.gameObject.CompareTag("DoubleInventoryDisplay"))
        {
            Debug.Log("Collision with DoubleInventoryDisplay detected");
            dualInventoryDisplayObject = true;
            singleInventoryDisplayObject = false;
        }
        else
        {
            Debug.Log("Collision with unknown object detected");
            singleInventoryDisplayObject = false;
            dualInventoryDisplayObject = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        singleInventoryDisplayObject = false;
        dualInventoryDisplayObject = false;
        closeSecondInventory?.Invoke();
        if(howManyOpen>=1)
        { howManyOpen--; }
    }
}