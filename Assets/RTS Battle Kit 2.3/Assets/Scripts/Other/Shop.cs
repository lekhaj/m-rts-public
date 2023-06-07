using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
	
	public GameObject shopButton;
	public GameObject buttonPanel;
	public GameObject unlockButton;
	public GameObject unitPrice;
	public Text gemsLabel;
	public Text unitNameLabel;
	public Text damageLabel;
	public Text healthLabel;
	public Text rangeLabel;
	public Text speedLabel;
	public Color unlockedColor;
	public Color unlockableColor;
	public Color lockedColor;
	public float gemAmount;
	public int maxDefeatGems;
	public int gemsVictory;
	public GameObject gemLabel;
	public GameObject unitsButton;
	
	[HideInInspector]
	public List<int> gemCounts = new List<int>();
	
	CharacterManager manager;
	int selected;
	GameObject selectedButton;
	float gems;
	bool assignedGems;

	void Start () {
		manager = GameObject.FindObjectOfType<CharacterManager>();
		
		if(manager == null)
			Destroy(gameObject);
		
		initializeShop();
		updateGemsLabel();
		closePanel();
		gemLabel.SetActive(false);
	}
	
	void Update(){
		if(Time.timeScale > 0)
			gems += Time.deltaTime * gemAmount;
		
		if(!assignedGems && (Manager.victory || Manager.gameOver)){
			if(Manager.victory){
				addGems(gemsVictory);
			}
			else{
				if((int)gems < maxDefeatGems){
					addGems((int)gems);
				}
				else{
					addGems(maxDefeatGems);
				}
			}
			
			assignedGems = true;
		}
		
		if(!Manager.StartMenu.activeSelf && unitsButton.activeSelf)
			unitsButton.SetActive(false);
	}
	
	void initializeShop(){
		GameObject firstButton = null;
		
		for(int i = 0; i < manager.troops.Count; i++){			
			//add a button to the list of buttons
			GameObject newButton = Instantiate(shopButton);
			RectTransform rectTransform = newButton.GetComponent<RectTransform>();
			rectTransform.SetParent(buttonPanel.transform, false);
			
			if(i == 0)
				firstButton = newButton;
			
			//set the correct button sprite
			newButton.GetComponent<Image>().sprite = manager.troops[i].buttonImage;
			
			if(gemCounts[i] <= 0)
				PlayerPrefs.SetInt("unit" + i, 1);
			
			if(!(PlayerPrefs.GetInt("unit" + i) != 0 || i == 0))
				newButton.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);		
			
			//set button name to its position in the list(important for the button to work later on)
			newButton.transform.name = "" + i;
			
			newButton.GetComponent<Button>().onClick.AddListener(
			() => { 
				changeSelection(int.Parse(newButton.transform.name), newButton); 
			}
			);
		}
		
		changeSelection(0, firstButton);
	}
	
	void changeSelection(int unit, GameObject button){
		selected = unit;
		selectedButton = button;
		
		foreach(Outline outline in GetComponentsInChildren<Outline>()){
			outline.enabled = false;
		}
		
		button.GetComponent<Outline>().enabled = true;
		
		Character character = manager.troops[unit].deployableTroops.GetComponent<Character>();
		
		if(character == null)
			character = manager.troops[unit].deployableTroops.transform.GetChild(0).GetComponent<Character>();
		
		unitNameLabel.text = manager.troops[unit].deployableTroops.name;
		damageLabel.text = character.damage + "";
		healthLabel.text = character.lives + "";
		rangeLabel.text = character.minAttackDistance + "";
		speedLabel.text = character.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().speed + "";
		
		if(PlayerPrefs.GetInt("unit" + unit) != 0 || unit == 0){
			unlockButton.GetComponent<Button>().enabled = false;
			unlockButton.GetComponent<Image>().color = unlockedColor;
			unlockButton.GetComponentInChildren<Text>().text = "Unlocked";
			unitPrice.SetActive(false);
		}
		else{			
			if(PlayerPrefs.GetInt("gems") >= gemCounts[unit]){
				unlockButton.GetComponent<Button>().enabled = true;	
				unlockButton.GetComponent<Image>().color = unlockableColor;
				unlockButton.GetComponentInChildren<Text>().text = "Unlock unit";
			}
			else{
				unlockButton.GetComponent<Button>().enabled = false;	
				unlockButton.GetComponent<Image>().color = lockedColor;
				unlockButton.GetComponentInChildren<Text>().text = "Locked";
			}
			
			unitPrice.SetActive(true);
			unitPrice.GetComponentInChildren<Text>().text = gemCounts[unit] + "";
		}
	}
	
	public void unlock(){
		PlayerPrefs.SetInt("gems", PlayerPrefs.GetInt("gems") - gemCounts[selected]);
		gemsLabel.text = PlayerPrefs.GetInt("gems") + "";
		PlayerPrefs.SetInt("unit" + selected, 1);
		
		unitPrice.SetActive(false);
		unlockButton.GetComponent<Button>().enabled = false;
		unlockButton.GetComponent<Image>().color = unlockedColor;
		unlockButton.GetComponentInChildren<Text>().text = "Unlocked";
		
		selectedButton.GetComponent<Image>().color = Color.white;
	}
	
	public void addGems(int gemCount){
		gemLabel.SetActive(true);
		gemLabel.GetComponentInChildren<Text>().text = "" + gemCount;
		PlayerPrefs.SetInt("gems", PlayerPrefs.GetInt("gems") + gemCount);
		gems = 0;
	}
	
	public void updateGemsLabel(){
		gemsLabel.text = PlayerPrefs.GetInt("gems") + "";
	}
	
	public void openPanel(){
		transform.Find("shop panel").gameObject.SetActive(true);
	}
	
	public void closePanel(){
		transform.Find("shop panel").gameObject.SetActive(false);
	}
}
