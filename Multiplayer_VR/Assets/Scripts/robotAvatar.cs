// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to manage the positioning of the player's avatars across the network
/// </summary>
public class robotAvatar : Photon.Pun.MonoBehaviourPun
{
    private GameObject headSet;

    bool avatarActive = true;

    public Transform headMesh;

    public Transform headObj;

    public Transform bodyObj;

    Vector3 headMotion = new Vector3();
    Vector3 bodyRotation = new Vector3();
    public float heightAdjustment;
    public Transform counterRotator;

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {


            Transform instTrans = ViveManager.m_instance.m_cam.transform;
            headObj.transform.position = instTrans.position;
            headObj.transform.rotation = instTrans.rotation;

            Vector3 xfer = new Vector3(-headObj.localEulerAngles.x, 0f, -headObj.localEulerAngles.z);
            counterRotator.localEulerAngles = xfer;

            bodyObj.position = new Vector3(headObj.transform.position.x, headObj.transform.position.y - heightAdjustment, headObj.transform.position.z);
            //bodyObj.eulerAngles = new Vector3(0f, headObj.transform.eulerAngles.y, 0f);

        }

        //if (avatarActive)
        //{
        //    //for the head
        //    headMotion = new Vector3(headSet.transform.eulerAngles.x, 0f, headSet.transform.eulerAngles.z);
        //    headObj.transform.localEulerAngles = headMotion;

        //    ///body
        //    bodyObj.position = new Vector3(headSet.transform.position.x, headSet.transform.position.y - heightAdjustment, headSet.transform.position.z);
        //    bodyObj.eulerAngles = new Vector3(0f, headSet.transform.eulerAngles.y, 0f);

        //}
        
    }




}
