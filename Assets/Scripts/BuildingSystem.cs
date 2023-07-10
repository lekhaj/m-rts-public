using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem instance;

    public GridLayout GridLayout;
    private Grid grid;
    [SerializeField]
    private Tilemap mainTileMap;  
    [SerializeField]
    private Tilemap tempTileMap;
    [SerializeField]
    private TileBase whiteTile;

    public GameObject CastleTomb;


    private PlaceableObject objectToPlace;

    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private Vector3 prevPos;
    private BoundsInt prevArea;

    public enum TileType
    {
        empty,
        white,
        green,
        red
    }

    #region Unity Methods

    private void Awake()
    {
        instance = this;
        grid = GridLayout.gameObject.GetComponent<Grid>();
    }

    private void Start()
    {
        string tilePath = @"Tiles\";
        tileBases.Add(TileType.empty, null);
        tileBases.Add(TileType.white, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.Add(TileType.green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.Add(TileType.red, Resources.Load<TileBase>(tilePath + "red"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InitializeWithObject(CastleTomb);
        }

        if (!objectToPlace)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            objectToPlace.Rotate();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Can be placed" + objectToPlace.CanBePlaced());
            if (objectToPlace.CanBePlaced())
            {
                objectToPlace.Place();
                //Vector3Int start = GridLayout.WorldToCell(objectToPlace.GetStartPosition());
                //TakeArea(start, objectToPlace.Size);
            }
            else
            {
                Destroy(objectToPlace.gameObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearArea();
            Destroy(objectToPlace.gameObject);
        }
    }

    #endregion

    #region Utils

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = GridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    private static TileBase[] GetTileBlocks(BoundsInt area, Tilemap tileMap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tileMap.GetTile(pos);
            counter++;
        }

        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    private static void FillTiles(TileBase[] arr, TileType type)
    {
        Debug.Log("Type" + type);
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }

    #endregion

    #region BuildingPlacement

    public void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Vector3.zero);

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        objectToPlace = obj.gameObject.GetComponent<PlaceableObject>();
        FollowBuilding();
        objectToPlace = obj.GetComponent<PlaceableObject>();
        obj.AddComponent<ObjectDrag>();
    }

    public bool CanBePlaced(BoundsInt area)
    {
        //BoundsInt area = new BoundsInt();
        //area.position = GridLayout.WorldToCell(objectToPlace.GetStartPosition());
        //area.size = placeableObject.Size;

        //TileBase[] baseArray = GetTileBlocks(area, mainTileMap);

        //foreach (var b in baseArray)
        //{
        //    if(b == whiteTile)
        //    {
        //        return false;
        //    }
        //}
        //return true;

        TileBase[] baseArray = GetTileBlocks(area, mainTileMap);
        foreach (var b in baseArray)
        {
            Debug.Log("bb" + b + " tile " + tileBases[TileType.white]);
            if(b.name.Trim() != tileBases[TileType.white].name.Trim())
            {
                Debug.Log("cannot be placed here");
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }


    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.empty, tempTileMap);
        SetTilesBlock(area, TileType.green, mainTileMap);
        //mainTileMap.BoxFill(start, whiteTile, start.x, start.y, start.x + size.x, start.y + size.y);
    }

    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.empty);
        tempTileMap.SetTilesBlock(prevArea, toClear);
    }

    public void FollowBuilding()
    {
        ClearArea();

        objectToPlace.area.position = GridLayout.WorldToCell(objectToPlace.gameObject.transform.position);
        BoundsInt buildArea = objectToPlace.area;

        //Debug.Log("Bounds" + buildArea);

        TileBase[] baseArray = GetTileBlocks(buildArea, mainTileMap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            Debug.Log("baseArray[" + i + "]: " + baseArray[i].name.Trim());
            Debug.Log("tileBases[TileType.white]: " + tileBases[TileType.white].name.Trim());
            if (baseArray[i].name.Trim() == tileBases[TileType.white].name.Trim())
            {
                tileArray[i] = tileBases[TileType.green];
            }
            else
            {
                FillTiles(tileArray, TileType.red);
                break;
            }
        }

        tempTileMap.SetTilesBlock(buildArea, tileArray);
        prevArea = buildArea;
    }

    #endregion

}
