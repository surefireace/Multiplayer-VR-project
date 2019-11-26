// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

/// <summary>
/// enemy ai
/// </summary>
public class Enemy : MonoBehaviourPun, IPunObservable
{
    [HideInInspector]
    public int destinationCounter = 0;

    List<Transform> destPoints = new List<Transform>();
    Transform lookAtObj;
    public GameObject MeshObj;
    public ParticleSystem baseSparks;
    public ParticleSystem zapParticle;
    public AudioSource zapAudio;
    bool attack = false;
    public Renderer[] meshObjects;

    private float m_currentHP = 0;
    [SerializeField] private float m_maxHP = 100;
    public Material redDamageMaterial;

    [HideInInspector]
    public GameObject[] tables;

    [SerializeField]
    private GameObject m_deathParticals;

    public Transform fireOrigin;
    public LineRenderer fireLine;

    // Use this for initialization
    void Start ()
    {
        m_currentHP = m_maxHP;

		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(m_currentHP <= 0 && photonView.IsMine)
        {
            Debug.LogFormat("<Color=Red>Enemy Dead</Color>");
            photonView.RPC("KillEnemy", RpcTarget.All);
        }
        if (!attack)
        {
            

        }

        if(MyGameManager.Instance.GameOver())
        {
            photonView.RPC("KillEnemy", RpcTarget.All);
        }

        transform.LookAt(lookAtObj.position);

    }

    public void addDestination(Transform dest)
    {

        destPoints.Add(dest);
        
    }

    public void nextDestination()
    {
        GetComponent<NavMeshAgent>().SetDestination(destPoints[destinationCounter].position);
        lookAtObj = destPoints[destinationCounter];

    }

    public void TakeDamage(float amount)
    {
        this.m_currentHP -= amount;

        StartCoroutine(flashRed());

    }

    IEnumerator flashRed()
    {
        //create an array that can hold two materials
        Material[] baseMtl = new Material[2];

        //the second material is whatever the ojbect's material is
        baseMtl[1] = GetComponent<Renderer>().material;

        //make the first material is red
        baseMtl[0] = redDamageMaterial;
            
        //assign the new material array to the object
        GetComponent<Renderer>().materials = baseMtl;

        yield return new WaitForSeconds(.5f);

        //new variable to gather all the materials, including the hightlight
        Material[] resetMtl = new Material[1];
        //get the base material
        resetMtl[0] = GetComponent<Renderer>().materials[1];
        //assign the material array without the outline
        GetComponent<Renderer>().materials = resetMtl;


        yield return null;
    }

    public void enemyAttack()
    {
        
        GetComponent<NavMeshAgent>().enabled = false;
        
        //Transform patrolPoint = transform;
        
        float patrolXRand = Random.Range(-2f, 2f);
        float patrolYRand = Random.Range(4f, 7f);
        float patrolZRand = Random.Range(-2f, 2f);

        Vector3 xfer = new Vector3(destPoints[destinationCounter].localPosition.x, destPoints[destinationCounter].localPosition.y, destPoints[destinationCounter].localPosition.z - 1.5f);

        Vector3 worldXfer = transform.TransformPoint(xfer);


        Vector3 patrolPoint = new Vector3(transform.position.x + patrolXRand, transform.position.y + patrolYRand, worldXfer.z);
        

        //GetComponent<Collider>().enabled = false;
        //MeshObj.GetComponent<Collider>().enabled = true;
        StartCoroutine(attackSequence(patrolPoint));
       

    }

    IEnumerator attackSequence(Vector3 point)
    {
        float speed = 0;
        float timeFactor = Random.Range(1f, 2f);
        Vector3 start = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        attack = true;

        float currentClosest = 100000;
        int currentClosestIndex = 0;
        int counter = 0;

        GameObject[] tableTargs = GameObject.FindGameObjectsWithTag("tableTarget");

        
        foreach (GameObject table in tableTargs)
        {
            float temp = Vector3.Distance(transform.position, table.transform.position);
            if (temp < currentClosest)
            {
                currentClosest = temp;
                currentClosestIndex = counter;
            }
            counter++;
        }
        



        lookAtObj = tableTargs[currentClosestIndex].transform;
        
        while (transform.position != point)
        {
            speed += Time.deltaTime * timeFactor;
            transform.position = Vector3.Lerp(start, point, speed);
            transform.LookAt(tables[currentClosestIndex].transform.position);
            yield return new WaitForEndOfFrame();
        }
        


        bool isFloating = true;
        float xXfer = 0;
        float yXfer = 0;

        float xBase = transform.position.x;
        float yBase = transform.position.y;
        StartCoroutine(attackTable(currentClosestIndex));

        
        while (isFloating)
        {
            xXfer = Mathf.PingPong(Time.deltaTime, 1);
            yXfer = Mathf.PingPong(Time.time, 1);
            //print("x is " + xXfer + " y is " + yXfer);
            transform.position = new Vector3((xBase + xXfer), (yBase + yXfer), transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        

        print("done");
        yield return null;
    }


    IEnumerator attackTable(int closestIndex)
    {
        int frameCounter = 0;
        int frameMax = Random.Range(200, 400);
        fireLine.enabled = true;
        fireLine.SetPosition(0, fireOrigin.position);
        fireLine.SetPosition(1, tables[closestIndex].transform.position);
        zapParticle.gameObject.transform.position = tables[closestIndex].transform.position;
        photonView.RPC("EnemyAttackSound", RpcTarget.All);  // play the sound for all clients


        while (frameCounter < frameMax)
        {
            fireLine.SetPosition(0, fireOrigin.position);
            frameCounter++;
            yield return new WaitForEndOfFrame();
        }




        MyGameManager.Instance.DisruptPrisms();
        fireLine.enabled = false;
        photonView.RPC("KillEnemy", RpcTarget.All); // kill the enemy for all clients
    }

    /// <summary>
    /// to play the attacking sound locally [DC]
    /// </summary>
    [PunRPC]
    private void EnemyAttackSound()
    {
        zapAudio.Play();
        zapParticle.Play();
        baseSparks.Play();

    }

    /// <summary>
    /// to kill the enemy localy [DC]
    /// </summary>
    [PunRPC]
    public void KillEnemy()
    {
        PhotonNetwork.Destroy(gameObject);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(m_deathParticals.name, gameObject.transform.position, gameObject.transform.rotation);
        }

    }

    /// <summary>
    /// to update the hp and actions of enemies across the network [DC]
    /// </summary>
    /// <param name="stream"> the data being sent/recived</param>
    /// <param name="info"> info on the data</param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.m_currentHP);
            stream.SendNext(this.attack);

        }
        else
        {
            // Network player, receive data
            this.m_currentHP = (float)stream.ReceiveNext();
            this.attack = (bool)stream.ReceiveNext();
            Debug.Log("hp: " + m_currentHP);

        }

    }

}
