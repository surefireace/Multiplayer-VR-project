// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

/// <summary>
/// manages the puzzle and its pieces. changes the color of the light beams based
/// on color that the piece is hit with.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LightPrisms : MonoBehaviour
{
    [SerializeField]
    private float m_dist = 10;

    private GameObject m_target;

    [SerializeField]
    private Color m_color = Color.white;
    private Color m_colorResult = Color.white;

    private LineRenderer m_lineR;

    /// <summary>
    /// for updateing the beam in the editor when the color is changed
    /// </summary>
    private void OnValidate()
    {
        m_lineR = GetComponent<LineRenderer>();
        m_lineR.SetPosition(0, transform.position);
        m_lineR.SetPosition(1, transform.position + transform.forward * m_dist);
        m_colorResult = m_color;
        SetLineRendererColor(m_color);
        Color t = m_color;
        t.a = .75f;
        GetComponent<Renderer>().sharedMaterial.color = t;
    }

    public Color Color
    {
        get
        {
            return m_color;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_lineR = GetComponent<LineRenderer>();
        m_lineR.SetPosition(0, transform.position);
        m_lineR.SetPosition(1, transform.position + transform.forward * m_dist);
    }

    // Update is called once per frame
    void Update()
    {
        m_lineR.SetPosition(0, transform.position);
        if (IsAlligned())
        {
            SetLineRendererColor(m_colorResult, false);
        }
        else
        {
            SetLineRendererColor(m_colorResult);
        }
    }

    /// <summary>
    /// sets the line renderer's color
    /// </summary>
    /// <param name="color"> the color its changing to</param>
    /// <param name="fade"> if to fade of the color</param>
    private void SetLineRendererColor(Color color, bool fade = true)
    {
        m_lineR.startColor = color;
        Color t = color;
        if (fade)
        {
            t.a = 0;
        }
        else
        {
            t.a = 1;
        }
        m_lineR.endColor = t;

    }

    /// <summary>
    /// changes the color based on the color passed in
    /// </summary>
    /// <param name="colorIn"> the color that is effecting the output</param>
    /// <returns>the new color based on color passed in</returns>
    public Color ChangeColor(Color colorIn)
    {
        m_lineR.enabled = true;
        Color colorOut = Color.white;

        if (colorIn == Color.white)
        {
            // do multiplicative color mixing to mimic light filters
            colorOut.r = Mathf.Clamp((colorIn.r * m_color.r), 0, 1);
            colorOut.g = Mathf.Clamp((colorIn.g * m_color.g), 0, 1);
            colorOut.b = Mathf.Clamp((colorIn.b * m_color.b), 0, 1);
        }
        else   
        {
            // do additve color mixing to mimic mixing paint
            colorOut.r = Mathf.Clamp((colorIn.r + m_color.r) / 2, 0, 1);
            colorOut.g = Mathf.Clamp((colorIn.g + m_color.g) / 2, 0, 1);
            colorOut.b = Mathf.Clamp((colorIn.b + m_color.b) / 2, 0, 1);
        }

        m_colorResult = colorOut;

        return colorOut;

    }

    /// <summary>
    /// checks to see if the pieces are aligned
    /// </summary>
    /// <returns>ture if the beam is hitting anouther prism</returns>
    private bool IsAlligned()
    {
        if(!m_lineR.enabled)
        {
            return false;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit rayHit;

        if(Physics.Raycast(ray, out rayHit, m_dist))
        {
            m_lineR.SetPosition(1, transform.position + transform.forward * Vector3.Distance(transform.position, rayHit.point));

            if (rayHit.collider.gameObject.tag == "Prism")
            {
                var tempLP = rayHit.collider.gameObject.GetComponent<LightPrisms>();
                if (tempLP != null)
                {
                    tempLP.ChangeColor(m_colorResult);
                    if(m_target != tempLP.gameObject)
                    {
                        if(m_target != null)
                        {
                            m_target.GetComponent<LineRenderer>().enabled = false;
                        }
                        m_target = tempLP.gameObject;
                    }
                }
                else
                {
                    if(rayHit.collider.gameObject.GetComponent<LightSolution>().CheckSolution(m_colorResult))
                    {
                        Debug.LogFormat("<Color=Green>Victory</Color>");
                        MyGameManager.Instance.EndGame(true);
                    }
                    else if (m_target != null)
                    {
                        var tempLR = m_target.GetComponent<LineRenderer>();

                        if (tempLR != null)
                        {
                            tempLR.enabled = false;
                        }
                        m_target.GetComponent<LightPrisms>().TurnChildOff(4);
                        m_target = null;
                    }
                }
                return true;
            }
        }
        else
        {
            m_lineR.SetPosition(1, transform.position + transform.forward * m_dist);
            if (m_target != null)
            {
                var tempLR = m_target.GetComponent<LineRenderer>();

                if (tempLR != null)
                {
                    tempLR.enabled = false;
                }
                m_target.GetComponent<LightPrisms>().TurnChildOff(4);
                m_target = null;
            }


        }
        return false;
    }

    /// <summary>
    /// to turn a previously aligned child off
    /// </summary>
    /// <param name="i"> the number of times called recursively</param>
    public void TurnChildOff(int i)
    {
        if(i <= 0)
        {
            return;
        }
        if (m_target != null)
        {
            var tempLR = m_target.GetComponent<LineRenderer>();

            if (tempLR != null)
            {
                tempLR.enabled = false;
            }
            m_target.GetComponent<LightPrisms>().TurnChildOff(--i);
            m_target = null;
        }
    }

    /// <summary>
    /// to randomize the colors of the prisms for when the enemies hit them
    /// </summary>
    /// <param name="color"> the new color they are switched to</param>
    public void Disrupt(Color color)
    {
        if (!MyGameManager.Instance.GameOver())
        {
            m_color = color;
            Color t = m_color;
            t.a = .75f;
            GetComponent<Renderer>().sharedMaterial.color = t;
            SetLineRendererColor(m_color);
        }

    }
}
