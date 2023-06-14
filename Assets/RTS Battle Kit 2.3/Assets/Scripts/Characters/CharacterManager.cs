using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//troop/unit settings
[System.Serializable]
public class Troop{
	public GameObject deployableTroops;
	public int troopCosts;
	public int foodCosts;
	public Sprite buttonImage;
	[HideInInspector]
	public GameObject button;
}

public class CharacterManager : MonoBehaviour {

	//variables visible in the inspector	
	public int startGold;
	public int startFood;
	public GUIStyle rectangleStyle;
	public ParticleSystem newUnitEffect;
	public Texture2D cursorTexture;
	public Texture2D cursorTexture1;
	public bool highlightSelectedButton;
	public Color buttonHighlight;
	public GameObject button;
	public float bombLoadingSpeed;
	public float BombRange;
	public GameObject bombExplosion;
	public KeyCode deselectKey;
	
	[Space(5)]
	public bool mobileDragInput;
	public bool originalPreviewSize;
	public int dragPreviewSize;
	
	[Space(10)]
	public List<Troop> troops;
	
	//variables not visible in the inspector
	public static Vector3 clickedPos;
	public static int gold;
	public static int food;
	public static GameObject target;
	
	private Vector2 mouseDownPos;
    private Vector2 mouseLastPos;
	private bool visible; 
    private bool isDown;
	private GameObject[] knights;
	private int selectedUnit;

	//Gold
	private GameObject goldText;
	private GameObject goldWarning;
	private GameObject addedGoldText;

	//Food
	private GameObject foodText;
	private GameObject foodWarning;
	private GameObject addedfoodText;

	//Tree
	private int maxTrees = 3;
	public static int treesCount = 0;
	private GameObject treeWarning;

	private GameObject characterList;
	private GameObject characterParent;
	private GameObject selectButton;
	
	private GameObject bombLoadingBar;
	private GameObject bombButton;
	private float bombProgress;
	private bool isPlacingBomb;
	private GameObject bombRange;
	private bool canDrag;
	private GameObject dragPreview;
	
	public static bool selectionMode;
	
	void Awake(){
		//find some objects
		characterParent = new GameObject("Characters");	
		selectButton = GameObject.Find("Character selection button");
		target = GameObject.Find("target");
		
		if(!GameObject.Find("Mobile"))
			mobileDragInput = false;
		
		if(mobileDragInput){
			highlightSelectedButton = false;
			GameObject.FindGameObjectWithTag("Mobile units button").SetActive(false);
		}
		
		characterList = GameObject.Find("Character buttons");
		
		//find text that displays your amount of gold
		goldText = GameObject.Find("gold text");
		//find text that displays your amount of food
		foodText = GameObject.Find("food text");
		//find text that appears when you get extra gold
		addedGoldText = GameObject.Find("added gold text");
		//find text that appears when you get extra food
		addedfoodText = GameObject.Find("added food text");
		
		//find warning that appears when you don't have enough gold to deploy troops
		goldWarning = GameObject.Find("gold warning");
		//find warning that appears when you don't have enough gold to deploy troops
		foodWarning = GameObject.Find("food warning");
		//find warning that appears when max tree count reach
		treeWarning = GameObject.Find("tree warning");
		
		//find bomb gameobjects
		bombLoadingBar = GameObject.Find("Loading bar");
		bombButton = GameObject.Find("Bomb button");
		bombRange = GameObject.Find("Bomb range");
	}

    private void OnEnable()
    {
		FoodProvider.AddFoodCount += AddFood;
	}

    void Start(){
		target.SetActive(false);
		//set cursor and add the character buttons
		Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
		
		//set gold amount to start gold amount
		gold = startGold;
		addedGoldText.SetActive(false);	
		goldWarning.SetActive(false);
		
		//set food amount to start food amount
		food = startFood;
		addedfoodText.SetActive(false);	
		foodWarning.SetActive(false);

		//set Tree
		treeWarning.SetActive(false);
		
		//play function addGold every five seconds
		InvokeRepeating("AddGold", 1.0f, 5.0f);
	
		bombRange.SetActive(false);
		isPlacingBomb = false;
	}
     
    void Update(){
		if(bombProgress < 1){
			//when bomb is loading, set red color and disable button
			bombProgress += Time.deltaTime * bombLoadingSpeed;
			bombLoadingBar.GetComponent<Image>().color = Color.red;
			bombButton.GetComponent<Button>().enabled = false;
		}
		else{
			//if bomb is done, set a blue color and enable the bomb button
			bombProgress = 1;
			bombLoadingBar.GetComponent<Image>().color = new Color(0, 1, 1, 1);
			bombButton.GetComponent<Button>().enabled = true;
		}
		
		//set fillamount to the bomb progress
		bombLoadingBar.GetComponent<Image>().fillAmount = bombProgress;
		
		//set gold text to gold amount
		goldText.GetComponent<UnityEngine.UI.Text>().text = "" + gold;
		foodText.GetComponent<UnityEngine.UI.Text>().text = "" + food;
		
		//ray from main camera
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
				
		//check if left mouse button gets pressed
        if(Input.GetMouseButtonDown(0)){

			//if you didn't click on UI and you have not enought gold, display a warning
			if(gold < troops[selectedUnit].troopCosts && !EventSystem.current.IsPointerOverGameObject()){
			StartCoroutine(GoldWarning());	
			}	
			if(food < troops[selectedUnit].foodCosts && !EventSystem.current.IsPointerOverGameObject()){
			StartCoroutine(FoodWarning());	
			}
			//check if you hit any collider when clicking (just to prevent errors)
			if(hit.collider != null){
				//if you click battle ground, if click doesn't hit any UI, if space is not down and if you have enough gold, deploy the selected troops and decrease gold amount
				if (hit.collider.gameObject.CompareTag("Battle ground") && selectionMode && !isPlacingBomb && !EventSystem.current.IsPointerOverGameObject()
				&& gold >= troops[selectedUnit].troopCosts && food >= troops[selectedUnit].foodCosts && (!GameObject.Find("Mobile") || (GameObject.Find("Mobile") && Mobile.deployMode))) {

					if (troops[selectedUnit].deployableTroops.CompareTag("Tree") && treesCount < maxTrees)
					{
						CreateUnit(hit);
					}
					else if (troops[selectedUnit].deployableTroops.CompareTag("Tree") && treesCount > maxTrees)
					{
						treeWarning.SetActive(true);
						DisappearTreeWarning();
					}
					else if (!troops[selectedUnit].deployableTroops.CompareTag("Tree"))
                    {
						CreateUnit(hit);
					}
				
			}
			
			//if you are placing a bomb and click...
			if(isPlacingBomb && !EventSystem.current.IsPointerOverGameObject()){
				//instantiate explosion
				Instantiate(bombExplosion, hit.point, Quaternion.identity);
				
				//reset bomb progress
				bombProgress = 0;
				isPlacingBomb = false;
				bombRange.SetActive(false);
				
				//find enemies
				GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

				foreach(GameObject enemy in enemies){
					if(enemy != null && Vector3.Distance(enemy.transform.position, hit.point) <= BombRange/2){
						//kill enemy if its within the bombrange
						enemy.GetComponent<Character>().lives = 0;	
					}
				}
			}
			else if(hit.collider.gameObject.CompareTag("Battle ground") && isPlacingBomb && EventSystem.current.IsPointerOverGameObject()){
				//if you place a bomb and click any UI element, continue but don't reset bomb progress
				isPlacingBomb = false;
				bombRange.SetActive(false);
			}
			}
        }
		
		if(hit.collider != null && isPlacingBomb && !EventSystem.current.IsPointerOverGameObject()){
			//show the bomb range at mouse position using a spot light
			bombRange.transform.position = new Vector3(hit.point.x, 75, hit.point.z);
			//adjust spotangle to correspond to bomb range
			bombRange.GetComponent<Light>().spotAngle = BombRange;
			bombRange.SetActive(true);
		}
		
		//if space is down too set the position where you first clicked
		if(Input.GetMouseButtonDown(0) && selectionMode && !isPlacingBomb 
		&& (!GameObject.Find("Mobile") || (GameObject.Find("Mobile") && !Mobile.selectionModeMove))){
		mouseDownPos = Input.mousePosition;
        isDown = true;
        visible = true;
		}
 
        // Continue tracking mouse position until mouse button is up
        if(isDown){
        mouseLastPos = Input.mousePosition;
			//if you release mouse button, remove rectangle and stop tracking
            if(Input.GetMouseButtonUp(0)){
                isDown = false;
                visible = false;
            }
        }
		
		//find and store all selectable objects (objects in scene with knight tag)
		knights = GameObject.FindGameObjectsWithTag("Knight");
		
		//if player presses d, deselect all characters
		if(Input.GetKeyDown(deselectKey)){
			foreach(GameObject knight in knights){
				if(knight != null){
					Character character = knight.GetComponent<Character>();
					character.selected = false;	
					character.selectedObject.SetActive(false);
				}
			}
		}
		
		//start selection mode when player presses spacebar
		if(Input.GetKeyDown("space")){
			selectCharacters();	
		}
		
		//for each button, check if we have enough gold to deploy the unit and color the button grey if it can not be deployed yet
		for(int i = 0; i < troops.Count; i++){
			if(troops[i].button != null){
				if(troops[i].troopCosts <= gold && troops[i].foodCosts <= food){
					troops[i].button.gameObject.GetComponent<Image>().color = Color.white;
				}
				else{
					troops[i].button.gameObject.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);
				}
			}
		}
		
		if(Input.GetMouseButtonUp(0) && mobileDragInput && canDrag){
			canDrag = false;
			Destroy(dragPreview);
			
			if(hit.collider != null && Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
				CreateUnit(hit);
		}
		
		if(Input.GetMouseButton(0) && canDrag && dragPreview != null){
			dragPreview.transform.position = Input.mousePosition;
			
			if(Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
				dragPreview.GetComponent<Image>().enabled = false;
			}
			else{
				dragPreview.GetComponent<Image>().enabled = true;
			}
		}
    }
	
	void CreateUnit(RaycastHit hit){
		Debug.Log("Create Unit");

		GameObject newTroop = Instantiate(troops[selectedUnit].deployableTroops, hit.point, troops[selectedUnit].deployableTroops.transform.rotation) as GameObject;
		Instantiate(newUnitEffect, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));

		//checking tree count
		if (newTroop.CompareTag("Tree"))
        {
			treesCount++;
        }

		newTroop.transform.parent = characterParent.transform;
		gold -= troops[selectedUnit].troopCosts;
		food -= troops[selectedUnit].foodCosts;
				
		foreach(Character character in GameObject.FindObjectsOfType<Character>()){
			if(character != this)
				character.findCurrentTarget();
		}
	}
 
    void OnGUI(){
		//check if rectangle should be visible
		if(visible){
            // Find the corner of the box
            Vector2 origin;
            origin.x = Mathf.Min(mouseDownPos.x, mouseLastPos.x);
			
            // GUI and mouse coordinates are the opposite way around.
            origin.y = Mathf.Max(mouseDownPos.y, mouseLastPos.y);
            origin.y = Screen.height - origin.y;  
			
            //Compute size of box
            Vector2 size = mouseDownPos - mouseLastPos;
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);   
			
            // Draw the GUI box
            Rect rect = new Rect(origin.x, origin.y, size.x, size.y);
            GUI.Box(rect, "", rectangleStyle);
			
			foreach(GameObject knight in knights){
			if(knight != null){
			Vector3 pos = Camera.main.WorldToScreenPoint(knight.transform.position);
			pos.y = Screen.height - pos.y;
			//foreach selectable character check its position and if it is within GUI rectangle, set selected to true
			if(rect.Contains(pos)){
			Character character = knight.GetComponent<Character>();
			character.selected = true;	
			character.selectedObject.SetActive(true);
			}
			}
			}
		}
    }
	
	//function to select another unit
	public void selectUnit(int unit){
		if(highlightSelectedButton){
			//remove all outlines and set the current button outline visible
			for(int i = 0; i < troops.Count; i++){
				if(troops[i].button != null)
					troops[i].button.GetComponent<Outline>().enabled = false;	
			}
			
			troops[unit].button.GetComponent<Outline>().enabled = true;
		}
		
		//selected unit is the pressed button
		selectedUnit = unit;
		
		if(mobileDragInput && !isPlacingBomb && gold >= troops[selectedUnit].troopCosts && food >= troops[selectedUnit].foodCosts){
			dragPreview = Instantiate(troops[unit].button);
			dragPreview.transform.SetParent(GameObject.FindObjectOfType<Settings>().gameObject.transform);
			
			if(!originalPreviewSize){
				dragPreview.GetComponent<RectTransform>().sizeDelta = new Vector2(dragPreviewSize, dragPreviewSize);
			}
			else{
				dragPreview.GetComponent<RectTransform>().sizeDelta = troops[unit].button.GetComponent<RectTransform>().sizeDelta;
			}
			
			Destroy(dragPreview.GetComponent<buttonStats>());
			canDrag = true;
		}
	}
	
	public void selectCharacters(){
		Debug.Log("select" + selectionMode);
		//turn selection mode on/off
		selectionMode = !selectionMode;
		if(selectionMode){
		//set cursor and button color to show the player selection mode is active
		selectButton.GetComponent<Image>().color = Color.red;	
		Cursor.SetCursor(cursorTexture1, Vector2.zero, CursorMode.Auto);
		if(GameObject.Find("Mobile")){
				Debug.Log("deplymode" + Mobile.deployMode);
			if(!Mobile.deployMode){
					Debug.Log("I am inside");
				GameObject.Find("Mobile").GetComponent<Mobile>().toggleDeployMode();
			}
			Mobile.camEnabled = false;
		}
		}
		else{
		//show the player selection mode is not active
		selectButton.GetComponent<Image>().color = Color.white;	
		Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
		
		//set target object false and deselect all units
		foreach(GameObject knight in knights){
		if(knight != null){
			Character character = knight.GetComponent<Character>();
			character.selected = false;	
			character.selectedObject.SetActive(false);
		}
		}
		target.SetActive(false);
		if(GameObject.Find("Mobile")){
			Mobile.camEnabled = true;
		}
		}
	}
	
	//warning if you need more gold
	IEnumerator GoldWarning(){
	if(!goldWarning.activeSelf){
	goldWarning.SetActive(true);
	
	//wait for 2 seconds
	yield return new WaitForSeconds(2);
	goldWarning.SetActive(false);
	}
	}	
	//warning if you need more food
	IEnumerator FoodWarning(){
	if(!foodWarning.activeSelf){
	foodWarning.SetActive(true);
	
	//wait for 2 seconds
	yield return new WaitForSeconds(2);
	foodWarning.SetActive(false);
	}
	}
	
	public void addCharacterButtons(){
		bool unitShop = (GameObject.Find("Unit shop") != null);
		
		//for all troops...
		for(int i = 0; i < troops.Count; i++){
			if(((i == 0 || PlayerPrefs.GetInt("unit" + i) == 1) && unitShop) || !unitShop){
				//add a button to the list of buttons
				GameObject newButton = Instantiate(button);
				RectTransform rectTransform = newButton.GetComponent<RectTransform>();
				rectTransform.SetParent(characterList.transform, false);
			
				//set button outline
				newButton.GetComponent<Outline>().effectColor = buttonHighlight;
			
				//set the correct button sprite
				newButton.gameObject.GetComponent<Image>().sprite = troops[i].buttonImage;
				//only enable outline for the first button
				if(i == 0 && highlightSelectedButton){
					newButton.GetComponent<Outline>().enabled = true;
				}
				else{
					newButton.GetComponent<Outline>().enabled = false;	
				}
			
				//set button name to its position in the list(important for the button to work later on)
				newButton.transform.name = "" + i;
			
				//set the button stats
				newButton.GetComponentInChildren<Text>().text = "Price: " + troops[i].troopCosts + 
				"\n Damage: " + troops[i].deployableTroops.GetComponentInChildren<Character>().damage + 
				"\n Lives: " + troops[i].deployableTroops.GetComponentInChildren<Character>().lives;
			
				//this is the new button
				troops[i].button = newButton;
			}
		}
	}
	
	public void placeBomb(){
		//start placing a bomb
		isPlacingBomb = true;
	}
	
	//functions which adds 100 to your gold amount and shows text to let player know
	void AddGold(){
	gold += 100;
	StartCoroutine(AddedGoldText());
	}

	void AddFood()
    {
		food += 50;
		StartCoroutine(AddedFoodText());
	}
	
	IEnumerator AddedGoldText(){
	addedGoldText.SetActive(true);	
	yield return new WaitForSeconds(0.7f);
	addedGoldText.SetActive(false);	
	}
	IEnumerator AddedFoodText(){
	addedfoodText.SetActive(true);	
	yield return new WaitForSeconds(0.7f);
	addedfoodText.SetActive(false);	
	}

	async void DisappearTreeWarning()
    {
		await Task.Delay(200);
		treeWarning.SetActive(false);
    }

    private void OnDisable()
    {
		FoodProvider.AddFoodCount -= AddFood;
    }
}