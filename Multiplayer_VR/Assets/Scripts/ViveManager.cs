// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// to manage the position of the vive head set and controllers
/// </summary>
public class ViveManager : MonoBehaviour
{
    public GameObject m_cam;
    public GameObject m_RHand;
    public GameObject m_RHand_gun;
    public GameObject m_bulletSpawn;
    public GameObject m_LHand;
    public GuideLineDrawer m_guideLineR;

    public static ViveManager m_instance = null;


    // Start is called before the first frame update
    void Awake()
    {
        if(m_instance == null)
        {
            m_instance = this;
        }
    }

    private void OnDestroy()
    {
        if (m_instance != null)
        {
            m_instance = null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
