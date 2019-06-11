using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEditor;

[Serializable]
public struct Intersection
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string X { get; set; }
    public string Y { get; set; }
    public string IncomingLanes { get; set; }
    public string InternalLanes { get; set; }
    public string Shape { get; set; }
}

/// <summary>
/// Junction class represents road network intersection. 
/// </summary>
public class Junction : MonoBehaviour
{
    private GameObject Junctions_GO;
    public Shader Road_Shader;
    public List<Intersection> Junction_List;
    public bool Built;

    // Start is called before the first frame update
    void Start()
    {
        Junction_List = new List<Intersection>();
        Junctions_GO = GameObject.Find("Junctions");
        Built = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Clear all current simulation data.
    /// </summary>
    public void ClearData()
    {
        Junction_List.Clear();
    }

    // Sumo shape sting to List of floats point order is
    // x1, y1, x2, y2, ....
    private List<float> ShapeStringToFloatList(string shape)
    {
        List<float> points = new List<float>();
        char[] find = new char[2];
        find[0] = ',';
        find[1] = ' ';
        string[] cuts = shape.Split(find);
        List<string> cutList = cuts.ToList();
        foreach (string cut in cutList)
        {
            points.Add(float.Parse(cut, CultureInfo.InvariantCulture.NumberFormat));
        }
        return points;
    }

    /// <summary>
    /// Build an Intersection.
    /// </summary>
    public void BuildJunction(Intersection inter, bool flat)
    {
        // String points to floats
        if (inter.Shape == null)
        {
            return;
        }
        List<float> fshape = ShapeStringToFloatList(inter.Shape);
        int numverts = fshape.Count() / 2;

        // Center of junction
        float xjunc = float.Parse(inter.X, CultureInfo.InvariantCulture.NumberFormat);
        float yjunc = float.Parse(inter.Y, CultureInfo.InvariantCulture.NumberFormat);
        Vector3 centerpoint = new Vector3(xjunc, yjunc, 0.1f);

        if (numverts > 5)
        {
            // Get Meshfilter and create a new mesh
            GameObject chunk = new GameObject();
            chunk.name = inter.Id;
            
            chunk.AddComponent<MeshRenderer>();
            Material m;
            if (flat)
            {
                m = new Material(Resources.Load("Materials/Road_Material")as Material);
            }
            else
            {
                m = new Material(Road_Shader);
            }
            
            chunk.GetComponent<MeshRenderer>().material = m;
            Mesh mesh = new Mesh();

            // Build Vertices
            Vector3[] verts = new Vector3[numverts + 1];
            int vc = 0;
            for (int i = 0; i < fshape.Count(); i = i + 2)
            {
                verts[vc] = new Vector3(fshape[i], 0.1f, fshape[i + 1]);
                vc++;
            }
            verts[verts.Length - 1] = new Vector3(centerpoint.x, 0.1f, centerpoint.y);
            mesh.vertices = verts;

            // Build Triangles
            int[] tris = new int[(verts.Length - 1) * 3];
            int triscounter = 0;
            int trisindex = 0;
            for (int j = 0; j < verts.Length - 1; j++)
            {
                tris[trisindex] = triscounter;
                tris[trisindex + 1] = triscounter + 1;
                tris[trisindex + 2] = verts.Length - 1;
                triscounter++;
                trisindex += 3;
            }
            tris[tris.Length - 1] = 0;
            tris[tris.Length - 2] = verts.Length - 2;
            tris[tris.Length - 3] = verts.Length - 1;
            mesh.triangles = tris;

            // Build Normals
            Vector3[] norms = new Vector3[numverts + 1];
            for (int k = 0; k < numverts + 1; k++)
            {
                norms[k] = -Vector3.up;
            }
            mesh.normals = norms;

            chunk.AddComponent<MeshFilter>().mesh = mesh;
            //chunk.transform.Translate(new Vector3(-1.5f, 0.0f, 0.0f), Space.Self);
            chunk.isStatic = true;

            chunk.transform.parent = Junctions_GO.transform;
            
        }
    }

    public void Build()
    {
        foreach (Intersection i in Junction_List)
        {
            BuildJunction(i, true);
        }

        Built = true;
    }
}

