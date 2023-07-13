using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField]
    private Buildings _building;

    public static BuildingPlacement Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector3 mouseWorldPosition = GetMouseWorldPos();
        //    if(mouseWorldPosition != Vector3.zero)
        //    {
        //        GameObject spawnedBuilding = Instantiate(_building.BuildingPrefab, mouseWorldPosition, Quaternion.identity);
        //        spawnedBuilding.AddComponent<ObjectDrag>();
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Cannot Be Spawned");
        //    }
        //}
    }

    #region GetMousePos

    public static Vector3 GetMouseWorldPos()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition);
        Debug.Log("Pos" + vec);
        return vec;
    }
  
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
            //Debug.Log("Coll" + hit.collider.CompareTag("SpawnArea"));
            //if (hit.collider.CompareTag("SpawnArea"))
            //}
            //{
            //else
            //{
            //    return Vector3.zero;
            //}
        }
        else
        {
            return Vector3.zero;
        }
    }
    #endregion



    public void ChangeSpawnBuilding(Buildings obj)
    {
        _building = obj;
        GameObject spawnedBuilding = Instantiate(_building.BuildingPrefab);
        spawnedBuilding.AddComponent<ObjectDrag>();
    }
}
