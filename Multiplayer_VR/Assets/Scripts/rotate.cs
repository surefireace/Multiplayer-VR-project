// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//testing
public class rotate : MonoBehaviour
{
    public Transform trackingObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = trackingObj.position;
        transform.Rotate(0, 0, 20);
    }
}
