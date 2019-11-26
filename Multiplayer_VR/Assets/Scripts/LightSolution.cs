// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// simple class to tell if the puzzle has been solved
/// </summary>
public class LightSolution : MonoBehaviour
{
    public Color m_solutionColor = Color.white;

    [SerializeField] private float m_tolerance = .01f;

    // Start is called before the first frame update
    void OnValidate()
    {
        GetComponent<Renderer>().sharedMaterial.color = m_solutionColor;
    }

    /// <summary>
    /// checks if the solution is correct
    /// </summary>
    /// <param name="color"> color that is being checked</param>
    /// <returns>true if matches within a tolerance</returns>
    public bool CheckSolution(Color color)
    {
        return Vector4.Distance(m_solutionColor, color) <= m_tolerance;
    }
}
