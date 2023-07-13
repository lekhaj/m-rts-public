using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 targetPosition;

    private PlaceableObject placeObj;

    public float lerpSpeed = 5f;

    private void Start()
    {
        placeObj = gameObject.GetComponent<PlaceableObject>();
    }

    private void Update()
    {
        if (!placeObj.Placed)
        {
            Vector3 pos = BuildingSystem.GetMouseWorldPosition();
            targetPosition = BuildingSystem.instance.SnapCoordinateToGrid(pos);
            BuildingSystem.instance.FollowBuilding();
        }

        transform.position = targetPosition;
    }

}
