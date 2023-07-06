using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointFollower : MonoBehaviour
{
    public GameObject[] wayPoints;
    int _currentWayPointIndex = 0;

    public float wayPointSpeed = 2f;

    void Update()
    {
        if (Vector3.Distance(transform.position, wayPoints[_currentWayPointIndex].transform.position) < 0.1f)
        {
            _currentWayPointIndex++;
            if (_currentWayPointIndex >= wayPoints.Length)
            {
                _currentWayPointIndex = 0;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, wayPoints[_currentWayPointIndex].transform.position, wayPointSpeed * Time.deltaTime);
        }

        if(transform.position.z < wayPoints[0].transform.position.z + 0.5)
        {
            Debug.Log("reached");
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else if(transform.position.z > wayPoints[1].transform.position.z - 0.5)
        {
                
        }
    }
}