using UnityEngine;
using System.Collections;

public class DeleteParticles : MonoBehaviour {
	
	//float visible in the inspector
	public float lifetime = 1f;
	
	void Start(){
		Debug.Log("Destroy");
	//Destroy gameobject (or particles) after lifetime
	Destroy(gameObject, lifetime);
	}
}
