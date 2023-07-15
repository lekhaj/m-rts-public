//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class UiManager : MonoBehaviour
//{
//    [SerializeField]
//    private GameObject _showBuildingPlacementUI;

//    private void OnEnable()
//    {
//        InputManager.Touch += ShowUI; 
//        InputManager.Exit += HideUI; 
//    }

//    private void ShowUI()
//    {
//        _showBuildingPlacementUI.SetActive(true);
//    }   
//    private void HideUI()
//    {
//        _showBuildingPlacementUI.SetActive(false);
//    }
//    private void OnDisable()
//    {
//        InputManager.Touch -= ShowUI;
//        InputManager.Exit -= HideUI;
//    }
//}
