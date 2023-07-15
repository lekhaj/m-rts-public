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

    public static bool placingTime = false;

    private void Start()
    {
        StopPlacement();
        
    }

    private void OnEnable()
    {
        InputManager.Touch += GetClickedPosition;
        //InputManager.Exit += HideUI;
    }

    private void GetClickedPosition(Vector3 itemToBePlacedPos)
    {
        _showBuildingPlacementUI.transform.position = new Vector3(itemToBePlacedPos.x, itemToBePlacedPos.y + 8f, itemToBePlacedPos.z);
        _showBuildingPlacementUI.SetActive(true);
        _cellIndicator.SetActive(false);
        if (!placingTime)
        {
            _buildingToBePlaced = itemToBePlacedPos;
            placingTime = true;
        }
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
        //if (inputManager.IsPointerOverUI())
        //{
        //    return;
        //}
        //Vector3 mousePosition = inputManager.GetMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(_buildingToBePlaced);
        GameObject newObject = Instantiate(_databaseSO.ObjectData[_selectedObjectIndex].Prefab);
        Vector3 worldPos = _grid.CellToWorld(gridPosition);
        Debug.Log("Building To be place New obj" + gridPosition + "pos" + worldPos);
        newObject.transform.position = worldPos;
        StopPlacement();
        //_gridVisulalization.SetActive(true);
        //_cellIndicator.SetActive(true);
        //InputManager.Touch += PlaceStructure;
        //InputManager.Exit += StopPlacement;
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
        placingTime = false;
        //_cellIndicator.SetActive(false);
        //InputManager.Touch -= PlaceStructure;
        //InputManager.Exit -= StopPlacement;
    }

    private void Update()
    {
        //if(_selectedObjectIndex < 0) { return; }
        Vector3 mousePosition  = inputManager.GetMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);
        //Debug.Log("MousePos" + mousePosition + "gridPos" + gridPosition);
        mouseIndicator.transform.position = mousePosition;
        _cellIndicator.transform.position = _grid.CellToWorld(gridPosition);
    }

    private void OnDisable()
    {
        InputManager.Touch -= GetClickedPosition;
    }
}
