﻿using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance;
    public static event Action OnClickOpenBuildingInventory;

    public GridLayout gridLayout;
    public Tilemap MainTileMap;
    public Tilemap TempTileMap;
    private Transform childMainTilemap;
    private Transform childTempTilemap;

    public static Dictionary<TileType, TileBase> tileDictionary = new Dictionary<TileType, TileBase>();


    private Building temp;
    private Vector3 prevPos;
    private BoundsInt prevArea;

    private void Awake()
    {
        GetMainTileMap();
        childMainTilemap.gameObject.SetActive(false);
        childTempTilemap.gameObject.SetActive(false);
        Instance = this;


    }
    private Transform GetMainTileMap()
    {
        GetTempTilemap();
       return childMainTilemap = transform.Find("MainTilemap");
    }
    private Transform GetTempTilemap()
    {

        return childTempTilemap = transform.Find("TempTilemap");
    }
    private void Start()
    {
        string tilePath = @"Prefabs/Placeable/";
        tileDictionary.Add(TileType.Empty, null);
        tileDictionary.Add(TileType.White, Resources.Load<TileBase>(tilePath + "White1"));
        tileDictionary.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "Green1"));
        tileDictionary.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "Red1"));
        if(MainTileMap == null || TempTileMap == null || gridLayout == null)
        {
            Debug.LogError("GridBuildingSystem: MainTileMap, TempTileMap or GridLayout is not assigned!");
            return;
        }

    }
    private void Update()
    {
        if (!temp)
        {
            return;
        }

        // Ciągłe podążanie za myszką
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cellPos = gridLayout.LocalToCell(mousePos);

        if (prevPos != cellPos)
        {
            temp.transform.position = gridLayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));
            prevPos = cellPos;
            FollowBuilding();
        }

        // Stawianie budynku na kliknięcie lub spację
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (temp.CanBePlaced())
            {
                temp.Place();
                childMainTilemap.gameObject.SetActive(false);
                childTempTilemap.gameObject.SetActive(false); // Dezaktywuj tymczasową mapę
                OnClickOpenBuildingInventory.Invoke();
                temp = null; // Wyczyść referencję po postawieniu
            }
        }
        // Anulowanie na Escape
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearArea();
            Destroy(temp.gameObject);
            childMainTilemap.gameObject.SetActive(false);
            childTempTilemap.gameObject.SetActive(false); // Dezaktywuj tymczasową mapę
            temp = null; // Wyczyść referencję po usunięciu
        }
    }
    void OnEnable()
    {
        // Rejestracja zdarzenia
        BuildingInventorySlot.OnItemChoosenFromBuildingMenu += InitializeWithBuilding;
    }
    void OnDisable()
    {
        // Wyrejestrowanie zdarzenia
        BuildingInventorySlot.OnItemChoosenFromBuildingMenu -= InitializeWithBuilding;
    }
    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] tiles = new TileBase[area.size.x * area.size.y * area.size.z];
        int index = 0;
        foreach (var position in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(position.x, position.y, 0);
            tiles[index] = tilemap.GetTile(position);
            index++;
        }
        return tiles;
    }
    private static void SetTilesBlock(BoundsInt area, TileType tileType, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, tileType);
        tilemap.SetTilesBlock(area, tileArray);
    }
    private static void FillTiles(TileBase[] tileArray, TileType tileType)
    {
        for (int i = 0; i < tileArray.Length; i++)
        {
            tileArray[i] = tileDictionary[tileType];
        }
    }
    //Trzeba przypisaæ do czegos co bedzie trigerowac czyli jakis obiekt podem dodany

    public void InitializeWithBuilding(GameObject building)
    {
        childMainTilemap.gameObject.SetActive(true); // Aktywuj tymczasową mapę
        childTempTilemap.gameObject.SetActive(true); // Aktywuj główną mapę
        temp = Instantiate(building, Vector3.zero, Quaternion.identity).GetComponent<Building>();
        FollowBuilding();
    }
    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[temp.area.size.x * temp.area.size.y * temp.area.size.z];
        FillTiles(toClear, TileType.Empty);
        TempTileMap.SetTilesBlock(temp.area, toClear);
    }
    private void FollowBuilding()
    {
        ClearArea();
        temp.area.position = gridLayout.WorldToCell(temp.gameObject.transform.position);
        BoundsInt buildingArea = temp.area;
        TileBase[] baseArray = GetTilesBlock(buildingArea, MainTileMap);
        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileDictionary[TileType.White])
            {
                tileArray[i] = tileDictionary[TileType.Green];
                Debug.Log("Tile set to Green at index: " + i);
            }
            else
            {
                FillTiles(tileArray, TileType.Red);
                break;
            }
        }
        TempTileMap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }
    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesBlock(area, MainTileMap);
        foreach (TileBase tile in baseArray)
        {
            if (tile != tileDictionary[TileType.White] && tile != tileDictionary[TileType.Green])
            {
                return false; // Można stawiać tylko na białych lub zielonych
            }
        }
        return true;
    }
    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, MainTileMap);
        SetTilesBlock(area, TileType.Green, TempTileMap);
    }
    public enum TileType
    {
        Empty,
        White,
        Green,
        Red
    }
}