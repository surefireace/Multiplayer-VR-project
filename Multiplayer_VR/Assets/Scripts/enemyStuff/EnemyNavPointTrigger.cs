// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// enemy navigation node trigger
/// </summary>
public class EnemyNavPointTrigger : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "enemy")
        {
            obj.gameObject.GetComponent<Enemy>().destinationCounter++;
            obj.gameObject.GetComponent<Enemy>().nextDestination();
            print("new");

        }


    }
}
