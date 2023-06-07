using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class dragUI : MonoBehaviour, IDragHandler{
	
	public enum DragObject{
		thisObject,
		parentObject,
		childObject,
		rootObject
	}
	
	public DragObject dragObject;
	
	RectTransform dragTransform;
	
	void Start(){
		switch(dragObject){
			case DragObject.thisObject: dragTransform = GetComponent<RectTransform>();; break;
			case DragObject.parentObject: dragTransform = transform.parent.gameObject.GetComponent<RectTransform>();; break;
			case DragObject.childObject: dragTransform = transform.GetChild(0).gameObject.GetComponent<RectTransform>();; break;
			case DragObject.rootObject: dragTransform = transform.root.gameObject.GetComponent<RectTransform>();; break;
		}
	}
     
    public void OnDrag(PointerEventData eventData){
		PointerEventData pointerData = eventData as PointerEventData;
        if(pointerData == null) 
			return;
 
		Vector2 currentPosition = dragTransform.position;
        currentPosition.x += pointerData.delta.x;
        currentPosition.y += pointerData.delta.y;
        dragTransform.position = currentPosition;
	}
}
