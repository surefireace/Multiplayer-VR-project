// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// simple class for a third observer for the game
/// </summary>
public class OverviewCam : MonoBehaviour
{

    public Camera m_overviewCam;
    public Camera m_otherCam;

    // Start is called before the first frame update
    void Start()
    {
        if (m_overviewCam == null)
        {
            Debug.LogWarning("error no overviewCam");
        }

        m_overviewCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            m_overviewCam.enabled = !m_overviewCam.enabled;
            m_otherCam.enabled = !m_otherCam.enabled;
        }

    }
}
