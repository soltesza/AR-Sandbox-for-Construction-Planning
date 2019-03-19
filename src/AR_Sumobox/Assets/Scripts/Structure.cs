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
public struct Poly
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Color { get; set; }
    public string Shape { get; set; }
}

public class Structure : MonoBehaviour
{
    private GameObject Structures_GO;
    public List<Poly> Polys;
    public Shader Concrete_Shader;
    public Shader Building_Shader;

    public void ClearData()
    {
        Polys.Clear();
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

    // Start is called before the first frame update
    void Start()
    {
        Structures_GO = GameObject.Find("Structures");
        Polys = new List<Poly>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
                //m = Resources.Load("Materials/Concrete_Material", typeof(Material)) as Material;
                m = new Material(Building_Shader);
            }
            else
            {
                m = Resources.Load("Materials/Concrete_Material", typeof(Material)) as Material;
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
