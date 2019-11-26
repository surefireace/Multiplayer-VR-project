// By Phillip Kauffold
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// timed destruction for enemy stuff
/// </summary>
public class timedDestroy : MonoBehaviour    
{
    public float delayTime;
    public AudioSource m_deathSound = null;
    public bool m_playSound = true;
    // Start is called before the first frame update
    void Start()
    {
        if (m_playSound)
        {
            m_deathSound?.Play();
        }
        StartCoroutine(destroyMe());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator destroyMe()
    {
        yield return new WaitForSeconds(delayTime);
        PhotonNetwork.Destroy(gameObject);

    }
}
