using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObject : MonoBehaviour
{
    public int ID;
    public Vector2Int Size = Vector2Int.one;

    private Grid _grid;

    private MeshRenderer _mesh;


    public bool CanBePlaced;


    public static event Action<GameObject, Vector3Int, Vector2Int, int> RemoveData;

    public Vector3Int GridPosition;

    private void Awake()
    {
        _mesh =  GetComponent<MeshRenderer>();
        _mesh.enabled = false;
    }

    private void Start()
    {
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        Debug.Log("Grid Poss" + GridPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Border"))
        {
            Debug.LogError("This building cannot be spawned here");
            Destroy(transform.parent.gameObject);
        }
        else
        {
            CanBePlaced = true;
            _mesh.enabled = true;
        }
        // Add all contact points of the collision to the list
    }
    private void OnDestroy()
    {
        Vector2Int size = Size;
        int id = ID;
        RemoveData?.Invoke(this.gameObject, GridPosition, size, id);
    }
}


