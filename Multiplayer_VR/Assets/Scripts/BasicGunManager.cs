// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

/// <summary>
/// simple script to network the position and rotaion for the gun
/// </summary>
public class BasicGunManager : MonoBehaviourPun
{
    // Update is called once per frame
    void Update()
    {
        if(!PhotonNetwork.IsMasterClient && !MyGameManager.Instance.m_observer)
        {
            Transform instTrans = ViveManager.m_instance.m_RHand_gun.transform;
            photonView.RPC("SyncGun", RpcTarget.All, instTrans.position, instTrans.rotation);
            //gameObject.transform.position = instTrans.position;
            //gameObject.transform.rotation = instTrans.rotation;
        }
    }

    /// <summary>
    /// sync the gun for the gun's position locally for the others in the game
    /// </summary>
    /// <param name="pos"> position of the gun</param>
    /// <param name="rot"> rotation of the gun</param>
    [PunRPC]
    public void SyncGun(Vector3 pos, Quaternion rot)
    {
        if (!MyGameManager.Instance.m_observer)
        {
            gameObject.transform.position = pos;
            gameObject.transform.rotation = rot;
        }
    }
}
