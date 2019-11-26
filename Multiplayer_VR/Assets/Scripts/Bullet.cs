// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// simple bullet script to keep track of TTL and collision
/// </summary>
public class Bullet : MonoBehaviourPun
{
    [Tooltip("life time is in seconds")]
    public float m_lifeTime = 5;
    private float m_damage = 0;
    private float m_timer = 0;

    public float Dammage
    {
        set
        {
            m_damage = value;
        }
    }

    /// <summary>
    /// collision for the bullet
    /// </summary>
    /// <param name="collision"> the collider of the hit object</param>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarningFormat(collision.gameObject.name + "  " + collision.gameObject.tag);

        if (collision.gameObject.tag == "enemy")
        {
            Debug.LogWarningFormat("<Color=Blue>Dammage delt </Color>");

            collision.gameObject.GetComponent<Enemy>()?.TakeDamage(m_damage);

        }
        if (photonView != null && photonView.IsMine)
        {
            GameObject parentTemp = gameObject.transform.parent.gameObject;
            PhotonNetwork.Destroy(gameObject.transform.parent.gameObject);
            if (parentTemp != null)
            {
                Destroy(parentTemp);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>()?.Play();
       
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        if(photonView != null && photonView.IsMine && m_timer >= m_lifeTime )
        {
            GameObject parentTemp = gameObject.transform.parent.gameObject;
            PhotonNetwork.Destroy(gameObject.transform.parent.gameObject);
            if(parentTemp != null)
            {
                Destroy(parentTemp);
            }
        }
    }
}
