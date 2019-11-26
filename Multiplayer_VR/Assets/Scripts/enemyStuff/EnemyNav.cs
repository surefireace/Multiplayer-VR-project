// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// enemy navigation
/// </summary>
public class EnemyNav : MonoBehaviour
{
    public Transform[] spawnPoints;

    public Transform[] navPoints01;

    public Transform[] navPoints02;

    public Transform[] endPoints;

    public Transform[] obstacles;

    public GameObject enemyPrefab;

    public GameObject[] tables;

    public ParticleSystem spawnEffect;


    // Start is called before the first frame update
    void Start()
    {
        meshOff(spawnPoints);
        meshOff(navPoints01);
        meshOff(navPoints02);
        meshOff(endPoints);
        //meshOff(obstacles);

    }
    void meshOff(Transform[] objOff)
    {

        foreach (Transform off in objOff)
            off.GetComponent<Renderer>().enabled = false;


    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnEnemyXfer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(spawnEnemy());
        }
    }

    IEnumerator spawnEnemy()
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        int navPoint01Index = Random.Range(0, navPoints01.Length);
        int navPoint02Index = Random.Range(0, navPoints02.Length);
        int endPointIndex = Random.Range(0, endPoints.Length);
        PhotonNetwork.Instantiate(spawnEffect.name, spawnPoints[spawnPointIndex].position, spawnEffect.transform.rotation);
        //spawnEffectInst.Play();
        
        // spawn the enemy across all clients [DC]
        GameObject enemyInst = PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation) as GameObject;
        
        //yield return new WaitForSeconds(1f);
        enemyInst.GetComponent<Enemy>().tables = tables;
        enemyInst.GetComponent<Enemy>().addDestination(navPoints01[navPoint01Index]);
        enemyInst.GetComponent<Enemy>().addDestination(navPoints02[navPoint02Index]);
        enemyInst.GetComponent<Enemy>().addDestination(endPoints[endPointIndex]);

        enemyInst.GetComponent<Enemy>().nextDestination();

        yield return null;


    }
}
