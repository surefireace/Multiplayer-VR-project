// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun.Demo.PunBasics;

/// <summary>
/// manages the enemies
/// </summary>
public class EnemiesManager : MonoBehaviour
{

    public EnemyNav[] sides;
    [HideInInspector]
    public bool stopSpawn = false;
    public GameObject[] tables;

    // Use this for initialization
    void Start ()
    {
        foreach (EnemyNav side in sides)
        {
            side.tables = tables;
        }

        StartCoroutine(startSpawn());
       

    }


    IEnumerator startSpawn()
    {
        yield return new WaitForSeconds(5f);
        newEnemy();
        yield return new WaitForSeconds(1f);
        newEnemy();

        StartCoroutine(timedEnemySpawn());

    }

    IEnumerator timedEnemySpawn()
    {
        while (!stopSpawn)
        {

            float delay = Random.Range(8f, 15f);
            yield return new WaitForSeconds(delay);
            newEnemy();
        }




    }


	
	// Update is called once per frame
	void Update ()
    {

        if (MyGameManager.Instance.GameOver())
        {
            StopCoroutine(timedEnemySpawn());
        }

    }

    public void newEnemy()
    {
        int sideIndex = Random.Range(0, sides.Length);

        //sides[sideIndex].tables = tables;
        sides[sideIndex].spawnEnemyXfer();

    }


}
