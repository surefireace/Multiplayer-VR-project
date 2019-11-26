// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// rotates the rings in the scene
/// </summary>
public class rotateRings : MonoBehaviour

{
    public float xRotation;
    public float yRotation;
    public float zRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(xRotation, yRotation, zRotation);
    }
}
