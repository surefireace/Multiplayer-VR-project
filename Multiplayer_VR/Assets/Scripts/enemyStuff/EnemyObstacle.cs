// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// enemy obstacles
/// </summary>
public class EnemyObstacle : MonoBehaviour {

    public float movementRange;
    public float speed;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = new Vector3(Mathf.PingPong(Time.time * speed, movementRange), 0f, 0f);
    }
}
