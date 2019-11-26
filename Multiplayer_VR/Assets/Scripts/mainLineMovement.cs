// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// simple class for the light for the puzzle to give it a visual effect
/// </summary>
public class mainLineMovement : MonoBehaviour
{

    LineRenderer line;
    Material lineMtl;
    float offset;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        lineMtl = line.material;
    }

    // Update is called once per frame
    void Update()
    {
        offset = offset + speed;
        lineMtl.mainTextureOffset = new Vector2(offset, 0);
    }
}
