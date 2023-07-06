using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseAreaEnd : MonoBehaviour {
	
	public Vector3 center;
	public Vector3 area;

	void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, area);
    }
}
