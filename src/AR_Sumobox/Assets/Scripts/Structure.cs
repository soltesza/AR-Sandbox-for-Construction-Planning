using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Poly struct holds polygon data that represents arbitrary network shapes.
/// </summary>
[Serializable]
public struct Poly
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Color { get; set; }
    public string Shape { get; set; }
}

/// <summary>
/// Structure class stores and builds all simulation network buildings and Points of Interest.
/// </summary>
public class Structure : MonoBehaviour
{
    /// <summary>
    /// The Structures main Game Object.
    /// </summary>
    private GameObject Structures_GO;
    /// <summary>
    /// The list of polygon data.
    /// </summary>
    public List<Poly> Polys;
    /// <summary>
    /// The parking lot shader.
    /// </summary>
    public Shader Road_Shader;
    /// <summary>
    /// The building extrusion shader.
    /// </summary>
    public Shader Building_Shader;
    /// <summary>
    /// Some extra colors for polygons.
    /// </summary>
    private Color[] BuildingColors = new Color[4];

    /// <summary>
    /// Clear all current simulation polygon data.
    /// </summary>
    public void ClearData()
    {
        Polys.Clear();
    }

    /// <summary>
    /// Sumo shape sting to List of floats 
    /// </summary>
    /// <param name="shape"></param>
    /// <returns>A list of floats, point order is x1, y1, x2, y2, ....</returns>
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

    // Start is called before the first frame update
    void Start()
    {
        Structures_GO = GameObject.Find("Structures");
        Polys = new List<Poly>();
        BuildingColors[0] = new Color(153.0f / 255.0f, 102.0f / 255.0f, 51.0f / 255.0f, 1.0f);
        BuildingColors[1] = new Color(102.0f / 255.0f, 51.0f / 255.0f, 0.0f / 255.0f, 1.0f);
        BuildingColors[2] = new Color(153.0f / 255.0f, 153.0f / 255.0f, 102.0f / 255.0f, 1.0f);
        BuildingColors[3] = new Color(153.0f / 255.0f, 51.0f / 255.0f, 0.0f / 255.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Build all stored polygon data.
    /// </summary>
    public void Build()
    {
        foreach (Poly p in Polys)
        {
            bool building = false;
            GameObject chunk = new GameObject();
            chunk.name = p.Id;
            MeshRenderer mr = chunk.AddComponent<MeshRenderer>();

            Material m;
            if (p.Type.Contains("building"))
            {
                building = true;
                m = new Material(Building_Shader);
                System.Random rnd = new System.Random();
                int bc = rnd.Next(1, 4) - 1;
                m.color = BuildingColors[bc];
            }
            else
            {
                m = new Material(Resources.Load("Materials/Road_Material") as Material);
                List<float> color;
                if (p.Color != null)
                {
                    color = ShapeStringToFloatList(p.Color);
                    m.color = new Color(color[0] / 255.0f, color[1] / 255.0f, color[2] / 255.0f, 1.0f);
                }
                
            }

            mr.material = m;
            List<float> pshape = ShapeStringToFloatList(p.Shape);

            List<Vector2> vecs = new List<Vector2>();
            for (int i = 0; i < pshape.Count-2; i+=2)
            {
                vecs.Add(new Vector2(pshape[i],pshape[i+1]));
            }

            Triangulator tr = new Triangulator(vecs.ToArray());
            int[] indices = tr.Triangulate();

            Vector3[] verts = new Vector3[vecs.Count];
            for (int j = 0; j < vecs.Count; j++)
            {
                if (building)
                {
                    verts[j] = new Vector3(vecs[j].x, 0.11f, vecs[j].y);
                }
                else
                {
                    verts[j] = new Vector3(vecs[j].x, 0.09f, vecs[j].y);
                }
                
            }

            Vector3[] norms = new Vector3[vecs.Count];
            for (int k = 0; k < vecs.Count; k++)
            {
                norms[k] = Vector3.up;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verts;
            mesh.triangles = indices;
            mesh.normals = norms;
            mesh.RecalculateBounds();
            MeshFilter mf = chunk.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            chunk.transform.parent = Structures_GO.transform;
        }
    }
}
