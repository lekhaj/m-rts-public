using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
 
public class buttonStats : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
	
	public bool selectUnitOnClick = true;
	
	//not visible in the inspector
	private GameObject stats;
	private CharacterManager manager;
	
	void Start(){
		//find button stats and turn them off
		stats = transform.Find("Stats").gameObject;
		stats.SetActive(false);
		
		if(selectUnitOnClick)
			manager = GameObject.FindObjectOfType<CharacterManager>();
	}
	
    public void OnPointerEnter (PointerEventData eventData) {
		//set stats active on hover
        stats.SetActive(true);
    }
 
    public void OnPointerExit (PointerEventData eventData) {
		//hide stats on exit
        stats.SetActive(false);
    }
	
	public void OnPointerUp (PointerEventData eventData) {
		//hide stats on exit
        stats.SetActive(false);
    }
	
	public void OnPointerDown (PointerEventData eventData) {
		//hide stats on click
        stats.SetActive(false);
		
		if(selectUnitOnClick)
			manager.selectUnit(int.Parse(transform.name));
    }
}
