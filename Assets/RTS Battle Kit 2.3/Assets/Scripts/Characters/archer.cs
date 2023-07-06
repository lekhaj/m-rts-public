using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class archer : MonoBehaviour
{

	//variables visible in the inspector
	public GameObject arrow;
	public Transform arrowSpawner;
	public GameObject animationArrow;

	//not visible in the inspector
	private bool shooting;
	private bool addArrowForce;
	private GameObject newArrow;
	private float shootingForce;
	private Animator animator;
	private Vector3 randomTarget;
	private DefenseArea defenseArea;
	private Vector3 defensePosition;
	private bool calledOnce = false;
	private Animator[] animators;

	void Start()
	{
		defenseArea = GameObject.FindObjectOfType<DefenseArea>();
		animators = gameObject.GetComponentsInChildren<Animator>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		//only shoot when animation is almost done (when the character is shooting)
		if (animator.GetBool("Attacking") == true && animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.95f && !shooting)
		{
			StartCoroutine(shoot());
		}

		//set an extra arrow active to make illusion of shooting more realistic
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.25f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.95f)
		{
			animationArrow.SetActive(true);
		}
		else
		{
			animationArrow.SetActive(false);
		}

		if (GetComponent<Character>().GoingToDefensePos && gameObject.CompareTag("Knight"))
		{
			Debug.Log("Going inside");
			if (defenseArea != null && GetComponent<NavMeshAgent>() != null)
			{
				if (randomTarget == Vector3.zero || Vector3.Distance(transform.position, randomTarget) < 3f)
					randomTarget = GetComponent<Character>().getRandomPositionInDefenseArea(defenseArea);

				// Set the defense position as the new destination
				defensePosition = randomTarget;
				Debug.Log("defense" + defensePosition);
				if (randomTarget != Vector3.zero)
				{
					Debug.Log("Movingg");
					if (animators[0].GetBool("Attacking"))
					{
						foreach (Animator animator in animators)
						{
							animator.SetBool("Attacking", false);
						}

						if (GetComponent<Character>().Source.clip != GetComponent<Character>().runAudio)
						{
							GetComponent<Character>().Source.clip = GetComponent<Character>().runAudio;
							GetComponent<Character>().Source.Play();
						}
					}

					GetComponent<NavMeshAgent>().isStopped = false;
					GetComponent<NavMeshAgent>().destination = defensePosition;
				}
			}
		}
	}

	void LateUpdate()
	{
		//check if the archer shoots an arrow
		if (addArrowForce && this.gameObject != null && GetComponent<Character>().currentTarget != null && newArrow != null && arrowSpawner != null)
		{
			//create a shootingforce
			shootingForce = Vector3.Distance(transform.position, GetComponent<Character>().currentTarget.transform.position);
			//add shooting force to the arrow
			newArrow.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(new Vector3(0, shootingForce * 12 +
			((GetComponent<Character>().currentTarget.transform.position.y - transform.position.y) * 45), shootingForce * 55)));
			addArrowForce = false;
		}
		else if (addArrowForce && this.gameObject != null && newArrow != null && arrowSpawner != null)
		{
			//shoot with a different force when archer is attacking a castle
			shootingForce = Vector3.Distance(transform.position, GetComponent<Character>().castleAttackPosition);
			newArrow.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(new Vector3(0, shootingForce * 12 +
			((GetComponent<Character>().castleAttackPosition.y - transform.position.y) * 45), shootingForce * 55)));
			addArrowForce = false;
		}
	}

	IEnumerator shoot()
	{
		//archer is currently shooting
		shooting = true;

		//add a new arrow
		newArrow = Instantiate(arrow, arrowSpawner.position, arrowSpawner.rotation) as GameObject;
		newArrow.GetComponent<Arrow>().arrowOwner = this.gameObject;
		//shoot it using rigidbody addforce
		addArrowForce = true;

		//wait and set shooting back to false
		yield return new WaitForSeconds(0.5f);
		shooting = false;
	}
}