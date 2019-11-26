// By Donovan Colen
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Valve.VR;
using HTC.UnityPlugin.Vive;

/// <summary>
/// a class that allows the creation of a mesh that would represent
/// the size and position of a IRL object in the scene
/// </summary>
public class createMesh : MonoBehaviour
{

    //center up the pivot for each individual plane

    //still make a prefab out of all of them together.


    public Vector3[] planePoints;

    public string objectName = ("origin");
    Vector2[] newUV;
    int[] newTriangles = new int[6];
    public Material tempMtl;
    public GameObject m_rightDeviceTracker;
    public GuideLineDrawer m_leftGuidLine;
    public GameObject m_marker;
    private GameObject m_selectedObj;

    ///public Mesh mesh;


    private void Start()
    {
        planePoints = new Vector3[4];

    }

    /// <summary>
    /// creates the object based on posion of points given
    /// </summary>
    void makeObject () {

        //create the top set of Vector3 coordinates from the planePoints

        Vector3[] newVerts = new Vector3[planePoints.Length];
        int index = 0;
        foreach (Vector3 vert in planePoints)
        {
            newVerts[index] = vert;
            index++;
        }

        ///create a plane on the floor from the top coordinates

        index = 0;
        Vector3[] groundVerts = new Vector3[newVerts.Length];
        foreach (Vector3 vert in planePoints)
        {
            Vector3 groundVert = new Vector3(vert.x, 0f, vert.z);
            groundVerts[index] = groundVert;
            index++;
        }

        ////calculate the center 
        ///we may want to change this to having the center at the bottom. 
        ///if we do, all we have to do is set the y coordinate to 0
        Vector3 parentObjCenter = new Vector3();

        Vector3[] allVerts = new Vector3[groundVerts.Length + newVerts.Length];

        int allVertCount = 0;
       
        foreach (Vector3 vert in newVerts)
        {
            allVerts[allVertCount] = vert;
            allVertCount++;
        }

        foreach (Vector3 vert in groundVerts)
        {
            allVerts[allVertCount] = vert;
            allVertCount++;
        }




        parentObjCenter = calcCenter(allVerts);
        GameObject parentObj = new GameObject();
        parentObj.name = ("obstacle");
        parentObj.transform.position = parentObjCenter;

        /////////////////////////
        //create the top 
        GameObject topMesh = new GameObject();
        topMesh.name = ("top");
        makeMesh(topMesh, newVerts);
        topMesh.transform.parent = parentObj.transform;
        //topMesh.AddComponent<meshExporter>();
        ////AssetDatabase.CreateAsset(GetComponent<MeshFilter>().mesh, "Assets/testMesh.asset");
        //topMesh.GetComponent<meshExporter>().exportORama();
        ////////////////////////
        //create the bottom mesh

        GameObject groundMesh = new GameObject();
        groundMesh.name = ("ground");
        makeMesh(groundMesh, groundVerts);
        groundMesh.transform.parent = parentObj.transform;
        ////////////////////////
        //make sides
        ////////////////////////

        //side One
        string sideName = ("side1");
        Vector3[] sideVerts = new Vector3[4];
        sideVerts[0] = newVerts[0];
        sideVerts[1] = newVerts[1];
        sideVerts[2] = groundVerts[1];
        sideVerts[3] = groundVerts[0];

        makeSide(sideName, sideVerts, parentObj);

        //side two
        sideName = ("side2");
        sideVerts[0] = newVerts[0];
        sideVerts[1] = newVerts[3];
        sideVerts[2] = groundVerts[3];
        sideVerts[3] = groundVerts[0];

        makeSide(sideName, sideVerts, parentObj);

        //side three
        sideName = ("side3");
        sideVerts[0] = newVerts[3];
        sideVerts[1] = newVerts[2];
        sideVerts[2] = groundVerts[2];
        sideVerts[3] = groundVerts[3];

        makeSide(sideName, sideVerts, parentObj);


        //side four
        sideName = ("side4");
        sideVerts[0] = newVerts[2];
        sideVerts[1] = newVerts[1];
        sideVerts[2] = groundVerts[1];
        sideVerts[3] = groundVerts[2];

        makeSide(sideName, sideVerts, parentObj);

        //create an object at the origin and parent the parentObj to it
        //this way we can accurately place it in the scene.
        GameObject originObj = new GameObject();
        originObj.name = (objectName);
        parentObj.transform.parent = originObj.transform;


    }



    // Update is called once per frame
    // for input grabbing
    void Update ()
    {

        if (Input.GetKeyDown(KeyCode.Space) || ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Menu))
        {
            makeObject();
            Debug.Log("createing cube");

        }

        if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Pad))
        {
            if (m_selectedObj != null)
            {
                MoveMesh();
                Debug.Log("moved obj");
            }
        }


        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.FullTrigger))
        {
            for (int i = 0; i < planePoints.Length; ++i)
            {
                if (planePoints[i] == Vector3.zero)
                {
                    planePoints[i] = m_rightDeviceTracker.transform.position; //+ (-m_rightDeviceTracker.transform.up * 0.025f) + (m_rightDeviceTracker.transform.forward * 0.025f);
                    Instantiate(m_marker, planePoints[i], Quaternion.identity);

                    break;
                }
            }

        }


        if (m_leftGuidLine.isActiveAndEnabled)
        {
            var points = m_leftGuidLine.raycaster.BreakPoints;
            var startPoint = points[0];
            var endPoint = points[points.Count - 1];

            Ray ray = new Ray(startPoint, endPoint - startPoint);

            RaycastHit result;
            if(Physics.Raycast(ray, out result))
            {
                m_selectedObj = result.collider.gameObject;
            }
        }

        //if(ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Pad))
        //{
        //    if (m_selectedObj != null)
        //    {
        //        GameObject temp = m_selectedObj.transform.parent.gameObject;
        //        PrefabUtility.ApplyObjectOverride(temp, "Assets/Prefabs/" + temp.name.ToString(), InteractionMode.AutomatedAction);
        //        print("saved prefab");
        //    }
            
        //}


    }

    /// <summary>
    /// to alow adjusting the mesh
    /// </summary>
    private void MoveMesh()
    {
        Vector3 cent = calcCenter(planePoints);
        cent.y = planePoints[0].y / 2;

        m_selectedObj.transform.parent.transform.position = cent;
    }

    /// <summary>
    /// creates the mesh for the object
    /// </summary>
    /// <param name="meshObj"> the object the mesh is being made for</param>
    /// <param name="verts"> verts used for mesh creation</param>
    void makeMesh(GameObject meshObj, Vector3[] verts)
    {
        meshObj.AddComponent<MeshFilter>();
        meshObj.AddComponent<MeshRenderer>();
        meshObj.GetComponent<MeshRenderer>().material = tempMtl;


        Mesh mesh = new Mesh();
        meshObj.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = verts;


        newTriangles[0] = 0;
        newTriangles[1] = 1;
        newTriangles[2] = 3;

        newTriangles[3] = 1;
        newTriangles[4] = 2;
        newTriangles[5] = 3;


        
        mesh.triangles = newTriangles;
        

    }

    /// <summary>
    /// makes a side of the object
    /// </summary>
    /// <param name="sidename"> the name of the side created</param>
    /// <param name="verts"> verts for size and position of the side</param>
    /// <param name="parentObj"> the parent object for the side</param>
    void makeSide(string sidename, Vector3[] verts, GameObject parentObj)
    {
        GameObject sideMesh = new GameObject();
        sideMesh.name = sidename;
        sideMesh.AddComponent<MeshFilter>();
        sideMesh.AddComponent<MeshRenderer>();
        sideMesh.GetComponent<MeshRenderer>().material = tempMtl;

        Mesh mesh = new Mesh();
        sideMesh.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = verts;
        int[] newTris = new int[6];

        newTris[0] = 0;
        newTris[1] = 3;
        newTris[2] = 2;

        newTris[3] = 0;
        newTris[4] = 1;
        newTris[5] = 2;

        mesh.triangles = newTris;
        sideMesh.transform.parent = parentObj.transform;

    }

    /// <summary>
    /// calculates the center of the object
    /// </summary>
    /// <param name="allVerts"> the verts for the size of the object</param>
    /// <returns></returns>
    Vector3 calcCenter(Vector3[] allVerts)
    {
        //calculate x
        float xVal = 0;
        foreach (Vector3 x in allVerts)
            xVal = xVal + x.x;

        xVal = xVal / (float)allVerts.Length;

        //calculate y
        float yVal = 0;
        foreach (Vector3 y in allVerts)
            yVal = yVal + y.y;

        yVal = yVal / (float)allVerts.Length;

        //calculate z
        float zVal = 0;
        foreach (Vector3 z in allVerts)
            zVal = zVal + z.z;

        zVal = zVal / (float)allVerts.Length;

        Vector3 returnVal = new Vector3(xVal, yVal, zVal);

        return returnVal;


    }

}
