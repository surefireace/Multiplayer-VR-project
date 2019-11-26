using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pyramidSequencer : MonoBehaviour
{
    public Texture[] pyramidTextures;
    //public Texture[] secondaryTxt;
    int counter = 0;
    int secondaryCounter = 0;
    [HideInInspector]
    public bool stopFlag = false;
    public Material pyramidMaterial;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(changeImage());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator changeImage()
    {

        while (!stopFlag)
        {
            if (counter >= pyramidTextures.Length)
                counter = 0;

            pyramidMaterial.mainTexture = pyramidTextures[counter];
            pyramidMaterial.SetTexture("_EmissionMap", pyramidTextures[counter]);
            //if (secondaryCounter >= secondaryTxt.Length)
            //    secondaryCounter = 0;

           // pyramidMaterial.SetTexture("_DetailAlbedoMap", secondaryTxt[secondaryCounter]);

            counter++;
            secondaryCounter++;
            yield return new WaitForEndOfFrame();
        }


        
    }
}
