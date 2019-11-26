// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// simple class that updates the position and rotation of the player for the network
/// </summary>
public class BasicPlayerManager : MonoBehaviourPun
{
    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            Transform instTrans = ViveManager.m_instance.m_cam.transform;
            gameObject.transform.position = instTrans.position;
            gameObject.transform.rotation = instTrans.rotation;
        }
    }
}
