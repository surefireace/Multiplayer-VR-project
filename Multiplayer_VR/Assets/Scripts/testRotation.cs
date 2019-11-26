// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//testing
public class testRotation : MonoBehaviour

  
{

    public Transform baseObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 xfer = new Vector3(-baseObj.localEulerAngles.x, -baseObj.localEulerAngles.y, -baseObj.localEulerAngles.z);
        transform.localEulerAngles = xfer;
        
    }
}
