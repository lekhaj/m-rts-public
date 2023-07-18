using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField]
    private Camera _camera;

    private Vector3 _lastPosition;

    [SerializeField]
    private LayerMask _placementLayerMask;

    public static event Action<Vector3> Touch;
    public static event Action<GameObject> BuildingHit;
    public static event Action Exit;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {

        Debug.Log("Is buuiling ht" + IsBuildingHit());

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI() && !IsBuildingHit())
        {
            Vector3 clickedPosition = GetMapPosition();
            Touch?.Invoke(clickedPosition);
        }
        else if(Input.GetMouseButtonDown(0) && IsBuildingHit())
        {
            Debug.Log("Bilding hit" + GetBuildingGameObject());

            BuildingHit?.Invoke(GetBuildingGameObject());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit?.Invoke();
        }
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _camera.nearClipPlane;
        Ray ray = _camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, float.MaxValue, _placementLayerMask))
        {
            _lastPosition = hit.point;
            //Debug.Log("last pos " + _lastPosition);
        }
        return _lastPosition;
    }    
    public GameObject GetBuildingGameObject()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _camera.nearClipPlane;
        Ray ray = _camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, float.MaxValue))
        {
            if(hit.collider.gameObject.CompareTag("Ally Buildings"))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }        
    public bool IsBuildingHit()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _camera.nearClipPlane;
        Ray ray = _camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, float.MaxValue))
        {
            if(hit.collider.gameObject.CompareTag("Ally Buildings"))
            {
                return true;
            }
        }
        return false;
    }    


}
