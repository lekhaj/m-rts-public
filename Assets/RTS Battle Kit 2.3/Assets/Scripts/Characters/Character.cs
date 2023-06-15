using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine.AI;

public class Character : MonoBehaviour {
	
	//variables visible in the inspector
	public float lives;
	public float damage;
	public float heal;
	public float minAttackDistance;
	public float minHealDistance;
	public float castleStoppingDistance;
	public int addGold;
	public int addFood;
	public List<string> attackTag;
	public string healTag;
	public string attackCastleTag;
	public ParticleSystem dieParticles;
	public GameObject ragdoll;
	public AudioClip attackAudio;
	public AudioClip runAudio;
	
	//variables not visible in the inspector
	[HideInInspector]
	public bool selected;
	[HideInInspector]
	public Transform currentTarget;
	[HideInInspector]
	public Vector3 castleAttackPosition;
	
	[Space(10)]
	public bool wizard;
	public ParticleSystem spawnEffect;
	public GameObject skeleton;
	public int skeletonAmount;
	public float newSkeletonsWaitTime;
	
	private NavMeshAgent agent;
	private GameObject[] enemies;
	private GameObject[] allies;
	private GameObject health;
	private GameObject healthbar;
	
	[HideInInspector]
	public GameObject selectedObject;
	private bool goingToClickedPos;
	private float startLives;
	private float defaultStoppingDistance;
	private GameObject castle;
	private bool wizardSpawns;
	private Animator[] animators;
	private Vector3 targetPosition;
	private AudioSource source;

	private Vector3 randomTarget;
	private WalkArea area;
	
	private ParticleSystem dustEffect;
	
	void Start(){
		source = GetComponent<AudioSource>();
		
		//character is not selected
		selected = false;
		//selected character is not moving to clicked position
		goingToClickedPos = false;
		//find navmesh agent component
		agent = gameObject.GetComponent<NavMeshAgent>();
		animators = gameObject.GetComponentsInChildren<Animator>();
	
		//find objects attached to this character
		health = transform.Find("Health").gameObject;
		healthbar =	health.transform.Find("Healthbar").gameObject;
		health.SetActive(false);
		Transform selectedTransform = transform.Find("selected object");
		if (selectedTransform != null)
		{
			selectedObject = selectedTransform.gameObject;
			selectedObject.SetActive(false);
		}
	
		//set healtbar value
		healthbar.GetComponent<Slider>().maxValue = lives;
		startLives = lives;
		//get default stopping distance
		if (agent != null) {
			defaultStoppingDistance = agent.stoppingDistance;
		}


        //find the castle closest to this character
        if (!gameObject.CompareTag("Tree"))
        {
			findClosestCastle();
        }
	
		//if there's a dust effect (cavalry characters), find and assign it
		if(transform.Find("dust"))
			dustEffect = transform.Find("dust").gameObject.GetComponent<ParticleSystem>();
	
		//if this is a wizard, start the spawning loop
		if(wizard)
			StartCoroutine(spawnSkeletons());
		
		area = GameObject.FindObjectOfType<WalkArea>();
	}
	
	void Update(){

		if(lives >= startLives)
        {
			lives = startLives;
        }

		bool walkRandomly = true;

		//find closest castle
		if(castle == null){
			if(!gameObject.CompareTag("Tree") &&  !gameObject.CompareTag("Healer"))
				findClosestCastle();
		}
		else{
			walkRandomly = false;
		}
		
		if(lives != startLives){
			//only use the healthbar when the character lost some lives
			if(!health.activeSelf)
				health.SetActive(true);
			
			health.transform.LookAt(2 * transform.position - Camera.main.transform.position);
			healthbar.GetComponent<Slider>().value = lives;
		}
		
		//find closest enemy
		if(currentTarget == null){
			findCurrentTarget();	
		}
		else{
			walkRandomly = false;
		}
		
		//if character ran out of lives add blood particles, add gold and destroy character
		if(lives < 1){
			StartCoroutine(die());
		}
		
		//play dusteffect when running and stop it when the character is not running
		if(dustEffect && animators[0].GetBool("Attacking") == false && !dustEffect.isPlaying){
			dustEffect.Play();
		}
		if(dustEffect && dustEffect.isPlaying && animators[0].GetBool("Attacking") == true){
			dustEffect.Stop();
		}
		
		//check if character must go to a clicked position
		checkForClickedPosition();
		
		if(!goingToClickedPos && walkRandomly){
			if(area != null && agent != null){
				if(agent.stoppingDistance > 2)
					agent.stoppingDistance = 2;
			
				if(randomTarget == Vector3.zero || Vector3.Distance(transform.position, randomTarget) < 3f)
					randomTarget = getRandomPosition(area);
				
				if(randomTarget != Vector3.zero){
					if(animators[0].GetBool("Attacking")){
						foreach(Animator animator in animators){
							animator.SetBool("Attacking", false);
						}
						
						if(source.clip != runAudio){
							source.clip = runAudio;
							source.Play();
						}
					}
					
					agent.isStopped = false;
					agent.destination = randomTarget;
				}
			}
			
			return;
		}
		else if(agent.stoppingDistance != defaultStoppingDistance){
			agent.stoppingDistance = defaultStoppingDistance;
		}
		
		//first check if character is not selected and moving to a clicked position
		if(!goingToClickedPos){

			if(gameObject.CompareTag("Healer"))
            {
				if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.transform.position) < minHealDistance)
                {
					if (!wizardSpawns)
						agent.isStopped = false;

					agent.destination = currentTarget.position;
					if (Vector3.Distance(currentTarget.position, transform.position) <= agent.stoppingDistance)
					{
						Vector3 currentTargetPosition = currentTarget.position;
						currentTargetPosition.y = transform.position.y;
						transform.LookAt(currentTargetPosition);

						foreach (Animator animator in animators)
						{
							animator.SetBool("Heal", true);
						}

						Debug.Log("Current Target Health" + currentTarget.gameObject.GetComponent<Character>().lives + "Intial Health" + currentTarget.gameObject.GetComponent<Character>().startLives);

						if (currentTarget.gameObject.GetComponent<Character>().lives < currentTarget.gameObject.GetComponent<Character>().startLives)
						{
							currentTarget.gameObject.GetComponent<Character>().lives += Time.deltaTime * heal;
							Debug.Log("Current Target Health After Heal" + currentTarget.gameObject.GetComponent<Character>().lives);
						}
                        else
                        {
							findCurrentTarget();
							foreach (Animator animator in animators)
							{
								animator.SetBool("Heal", false);
							}
						}
					}

					//if its still traveling to the target, play running animation
					if (animators[0].GetBool("Heal") && Vector3.Distance(currentTarget.position, transform.position) > agent.stoppingDistance && !wizardSpawns)
					{
						foreach (Animator animator in animators)
						{
							animator.SetBool("Heal", false);
						}

						if (source.clip != runAudio)
						{
							source.clip = runAudio;
							source.Play();
						}
					}
				}
				//if currentTarget is out of range...
				else
				{
					//if character is not moving to clicked position attack the castle
					if (!goingToClickedPos)
					{
						if (!wizardSpawns)
							agent.isStopped = false;

						agent.destination = transform.position;
					}
				}


			}
            else
            {
				//If there's a currentTarget and its within the attack range, move agent to currenttarget
				if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.transform.position) < minAttackDistance)
				{
					if (!wizardSpawns)
						agent.isStopped = false;

					agent.destination = currentTarget.position;

					//check if character has reached its target and than rotate towards target and attack it
					if (Vector3.Distance(currentTarget.position, transform.position) <= agent.stoppingDistance)
					{
						Vector3 currentTargetPosition = currentTarget.position;
						currentTargetPosition.y = transform.position.y;
						transform.LookAt(currentTargetPosition);

						foreach (Animator animator in animators)
						{
							animator.SetBool("Attacking", true);
						}

						if (source.clip != attackAudio)
						{
							source.clip = attackAudio;
							source.Play();
						}

						currentTarget.gameObject.GetComponent<Character>().lives -= Time.deltaTime * damage;
					}

					//if its still traveling to the target, play running animation
					if (animators[0].GetBool("Attacking") && Vector3.Distance(currentTarget.position, transform.position) > agent.stoppingDistance && !wizardSpawns)
					{
						foreach (Animator animator in animators)
						{
							animator.SetBool("Attacking", false);
						}

						if (source.clip != runAudio)
						{
							source.clip = runAudio;
							source.Play();
						}
					}
				}
				//if currentTarget is out of range...
				else
				{
					//if character is not moving to clicked position attack the castle
					if (!goingToClickedPos)
					{
						if (!wizardSpawns)
							agent.isStopped = false;

						agent.destination = castleAttackPosition;
					}

					//if character is close enough to castle, attack castle
					if (castle != null && Vector3.Distance(transform.position, castleAttackPosition) <= castleStoppingDistance + castle.GetComponent<Castle>().size)
					{
						agent.isStopped = true;

						foreach (Animator animator in animators)
						{
							animator.SetBool("Attacking", true);
						}
						if (castle != null)
						{
							castle.GetComponent<Castle>().lives -= Time.deltaTime * damage;
						}

						if (source.clip != attackAudio)
						{
							source.clip = attackAudio;
							source.Play();
						}
					}

					//if character is traveling to castle play running animation
					else if (!wizardSpawns)
					{
						agent.isStopped = false;

						foreach (Animator animator in animators)
						{
							animator.SetBool("Attacking", false);
						}

						if (source.clip != runAudio)
						{
							source.clip = runAudio;
							source.Play();
						}
					}
				}
			}
			
		}
		//if character is going to clicked position...
		else{
			//if character is close enough to clicked position, let it attack enemies again
			if(Vector3.Distance(transform.position, targetPosition) < agent.stoppingDistance + 1){
				goingToClickedPos = false;	
				
				if(!wizardSpawns)
					agent.isStopped = false;
			}
		}
	}
	
	public void findClosestCastle(){
		//find the castles that should be attacked by this character
		GameObject[] castles = GameObject.FindGameObjectsWithTag(attackCastleTag);
		
		//distance between character and its nearest castle
		float closestCastle = Mathf.Infinity;
		
		foreach(GameObject potentialCastle in castles){
		//check if there are castles left to attack and check per castle if its closest to this character
			if(Vector3.Distance(transform.position, potentialCastle.transform.position) < closestCastle && potentialCastle != null){
			//if this castle is closest to character, set closest distance to distance between character and this castle
			closestCastle = Vector3.Distance(transform.position, potentialCastle.transform.position);
			//also set current target to closest target (this castle)
			castle = potentialCastle;
			}
		}	
		
		//Define a position to attack the castles(to spread characters when they are attacking the castle)
		if(castle != null)
			castleAttackPosition = castle.transform.position;
	}
	
	public void findCurrentTarget(){

        if (gameObject.CompareTag("Healer"))
        {
			allies = GameObject.FindGameObjectsWithTag("Knight");
			Debug.Log("allies" + allies[0]);

			GameObject targetAlly = null; 

			foreach (GameObject potentialTarget in allies) {

				Character character = potentialTarget.GetComponent<Character>();
				if (character != null && character.lives < character.startLives)
				{
					targetAlly = potentialTarget;
				}
			}
			// Set the targetAlly as the current target if it is different from the current target
			if (targetAlly != null && targetAlly.transform != currentTarget)
			{
				currentTarget = targetAlly.transform;
			}
		}
        else
        {
			List<GameObject> allEnemies = new List<GameObject>();
			//find all potential targets (enemies of this character)
			// Loop through each attack tag
			for (int i = 0; i < attackTag.Count; i++)
			{

				string tag = attackTag[i];

				// Find enemies with the current attack tag
				if (string.IsNullOrEmpty(tag))
					continue;

				enemies = GameObject.FindGameObjectsWithTag(tag);

				// Add the found enemies to the list of all enemies
				allEnemies.AddRange(enemies);
			}

			//distance between character and its nearest enemy
			float closestDistance = Mathf.Infinity;

			foreach (GameObject potentialTarget in allEnemies)
			{
				//check if there are enemies left to attack and check per enemy if its closest to this character
				if (Vector3.Distance(transform.position, potentialTarget.transform.position) < closestDistance && potentialTarget != null)
				{
					//if this enemy is closest to character, set closest distance to distance between character and enemy
					closestDistance = Vector3.Distance(transform.position, potentialTarget.transform.position);
					//also set current target to closest target (this enemy)
					if (!currentTarget || (currentTarget && Vector3.Distance(transform.position, currentTarget.position) > 2))
					{
						currentTarget = potentialTarget.transform;
					}
				}
			}
		}
	}
	
	public Vector3 getRandomPosition(WalkArea area){
		Vector3 center = area.center;
		Vector3 bounds = area.area;
		float yRay = center.y + bounds.y/2f;
		
		Vector3 rayStart = new Vector3(center.x + Random.Range(-bounds.x/2f, bounds.x/2f), yRay, center.z + Random.Range(-bounds.z/2f, bounds.z/2f));
		RaycastHit hit;
		
		if(Physics.Raycast(rayStart, -Vector3.up, out hit, bounds.y))
			return hit.point;
		
		return Vector3.zero;
	}
	
	public void checkForClickedPosition(){
	RaycastHit hit;
	
	//if character is selected and right mouse button gets clicked...
    if(selected && ((Input.GetMouseButtonDown(1) && !GameObject.Find("Mobile")) || (Input.GetMouseButtonDown(0) && GameObject.Find("Mobile") && Mobile.selectionModeMove))){
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if(Physics.Raycast(ray, out hit))
		//if you clicked battle ground, move character to clicked point and play running animation
		if(hit.collider.gameObject.CompareTag("Battle ground")){
		agent.isStopped = false;
        agent.SetDestination(hit.point);
		CharacterManager.clickedPos = hit.point;
		targetPosition = hit.point;
		goingToClickedPos = true;
			if(GetComponent<archer>() != null)
				agent.stoppingDistance = 2;

			foreach(Animator animator in animators){
				animator.SetBool("Attacking", false);
				animator.SetBool("Heal", false);

			}
			
			if(source.clip != runAudio){
				source.clip = runAudio;
				source.Play();
			}
						
			//set the yellow target position
			CharacterManager.target.transform.position = hit.point;
			CharacterManager.target.SetActive(true);
		}
    }	
	}
	
	//if this is a wizard:
	IEnumerator spawnSkeletons(){
		
		//stop the spawneffect
		spawnEffect.Stop();
		
		//create an infinite loop
		while(true){
			
			//if character is not attacking and there's no target in range...
			if(animators[0].GetBool("Attacking") == false && ((currentTarget != null && Vector3.Distance(transform.position, currentTarget.transform.position) >= minAttackDistance) || !currentTarget)){
			//wizard is now spawning
			wizardSpawns = true;
			
			//stop the navmesh agent
			agent.isStopped = true;
			
			//start spawning animation
			animators[0].SetBool("Spawning", true);
			//wait 0.5 seconds
			yield return new WaitForSeconds(0.5f);
			//play the spawn effect
			spawnEffect.Play();	
			
			//spawn the correct amount of skeletons with some delay
			for(int i = 0; i < skeletonAmount; i++){
			spawnSkeleton();
			yield return new WaitForSeconds(2f / skeletonAmount);
			}
			
			//stop playing the spawneffect
			spawnEffect.Stop();
			
			//wait for 0.5 seconds again
			yield return new WaitForSeconds(0.5f);
			
			//wizard is not spawning skeletons anymore
			wizardSpawns = false;
			
			//stop the spawning animation
			animators[0].SetBool("Spawning", false);
			}
		
		//short delay before the loop starts again
		yield return new WaitForSeconds(newSkeletonsWaitTime);
		}
	}
	
	public void spawnSkeleton(){
		//instantiate a skeleton character in front of the wizard
		Vector3 position = new Vector3(transform.position.x + Random.Range(1f, 2f), transform.position.y, transform.position.z + Random.Range(-0.5f, 0.5f));
		Instantiate(skeleton, position, Quaternion.identity);
	}
	
	public IEnumerator die(){

		if (gameObject.CompareTag("Tree"))
		{
			CharacterManager.treesCount--;
			Debug.Log("Count" + CharacterManager.treesCount);
		}

		if (ragdoll == null){
			Vector3 position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
			ParticleSystem particles = Instantiate(dieParticles, position, transform.rotation) as ParticleSystem;
			
			if(GameObject.Find("Manager") != null)
				particles.transform.parent = GameObject.Find("Manager").transform;
		}
		else{
			Instantiate(ragdoll, transform.position, transform.rotation);
		}
		
		CharacterManager.gold += addGold;
			
		if(gameObject.tag == "Enemy")
			Manager.enemiesKilled++;
		
		foreach(Character character in GameObject.FindObjectsOfType<Character>()){
			if(character != this)
				character.findCurrentTarget();
		}
		
		yield return new WaitForEndOfFrame();
		Destroy(gameObject);
	}
}
