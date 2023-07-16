using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator, _cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid _grid;

    [SerializeField]
    private ObjectsDatabaseSO _databaseSO;
    private int _selectedObjectIndex = -1;

    //[SerializeField]
    //private GameObject _gridVisulalization;
    [SerializeField]
    private GameObject _showBuildingPlacementUI;

    private Vector3 _buildingToBePlaced;

    private static bool _canBePlaced = true;

    private Renderer _previewRenderer;

    private GridData gridData;

    public static bool PlacingTime = false;

    private List<GameObject> _placedGameObjects = new();

    private void Start()
    {
        StopPlacement();
        _previewRenderer = _cellIndicator.GetComponentInChildren<Renderer>();
        gridData = new GridData();
    }

    private void OnEnable()
    {
        InputManager.Touch += GetClickedPosition;
        //InputManager.Exit += HideUI;
    }

    private void GetClickedPosition(Vector3 itemToBePlacedPos)
    {
        PlacingTime = true;
        Debug.Log("Item tobe placed" + itemToBePlacedPos);
        _showBuildingPlacementUI.transform.position = new Vector3(itemToBePlacedPos.x, itemToBePlacedPos.y + 8f, itemToBePlacedPos.z);
        _showBuildingPlacementUI.SetActive(true);
        if (InputManager.Instance.IsPointerOverUI())
            _cellIndicator.SetActive(false);
        _buildingToBePlaced = itemToBePlacedPos;

        Vector3Int gridPosition = _grid.WorldToCell(_buildingToBePlaced);
        Vector3 worldPos = _grid.CellToWorld(gridPosition);
        Debug.Log("Building To be place before" + gridPosition + "pos" + worldPos);
    }

    public void StartPlacement(int ID)
    {
        Debug.Log("Building To be place 11" + _buildingToBePlaced);
        _selectedObjectIndex = _databaseSO.ObjectData.FindIndex(data => data.ID == ID);
        if (_selectedObjectIndex < 0)
        {
            Debug.LogError("No Objects to be placed");
            return;
        }
        Vector3Int gridPosition = _grid.WorldToCell(_buildingToBePlaced);

        bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        if (!placementValidity)
        {
            return;
        }

        GameObject newObject = Instantiate(_databaseSO.ObjectData[_selectedObjectIndex].Prefab);
        Vector3 worldPos = _grid.CellToWorld(gridPosition);
        //Debug.Log("Building To be place New obj" + gridPosition + "pos" + worldPos);
        newObject.transform.position = worldPos;

        _placedGameObjects.Add(newObject);
        Debug.Log("Sizeee" + _databaseSO.ObjectData[_selectedObjectIndex].Size);
        gridData.AddObjectAt(gridPosition, _databaseSO.ObjectData[_selectedObjectIndex].Size, _databaseSO.ObjectData[_selectedObjectIndex].ID, _placedGameObjects.Count - 1);

        foreach (KeyValuePair<Vector3Int, PlacementData> kvp in gridData.placedObjects)
        {
            Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value);
        }

        StopPlacement();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //GridData selectedData = _databaseSO.ObjectData[selectedObjectIndex].ID == 0 ? _buildingsData : _troopsData;

        return gridData.CanPlaceObjectAt(gridPosition, _databaseSO.ObjectData[selectedObjectIndex].Size);
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);
        GameObject newObject = Instantiate(_databaseSO.ObjectData[_selectedObjectIndex].Prefab);
        newObject.transform.position = _grid.CellToWorld(gridPosition);
    }

    private void StopPlacement()
    {
        _selectedObjectIndex = -1;
        _showBuildingPlacementUI.SetActive(false);
        _cellIndicator.SetActive(true);
        PlacingTime = false;
        //_cellIndicator.SetActive(false);
        //InputManager.Touch -= PlaceStructure;
        //InputManager.Exit -= StopPlacement;
    }

    private void Update()
    {
        Vector3 mousePosition = inputManager.GetMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition); 

        bool canBePlaced = gridData.CanPlaceObjectAt(gridPosition);
        _previewRenderer.material.color = canBePlaced ? Color.green : Color.red;

        if (!canBePlaced)
        {
            _showBuildingPlacementUI.SetActive(false);
        }

        mouseIndicator.transform.position = mousePosition;
        _cellIndicator.transform.position = _grid.CellToWorld(gridPosition);

        
    }

    private void OnDisable()
    {
        InputManager.Touch -= GetClickedPosition;
    }
}
