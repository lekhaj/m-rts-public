using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offSet;
    private Vector3 targetPosition;

    public float lerpSpeed = 8f;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
    }

    private void OnMouseDown()
    {
        offSet = transform.position - BuildingSystem.GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        Vector3 pos = BuildingSystem.GetMouseWorldPosition() + offSet;
        targetPosition = BuildingSystem.instance.SnapCoordinateToGrid(pos);
    }
}
