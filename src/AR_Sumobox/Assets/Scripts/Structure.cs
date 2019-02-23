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
            GameObject chunk = new GameObject();
            MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
            if (p.Color != null)
            {
                List<float> col = ShapeStringToFloatList(p.Color);
                mr.material.color = new Vector4(col[0],col[1],col[2],1.0f);
            }
            else
            {
                mr.material.color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            }
            
            Mesh mesh = new Mesh();

            List<float> pshape = ShapeStringToFloatList(p.Shape);

            List<Vector3> vecs = new List<Vector3>();
            float xhigh, xlow, yhigh, ylow;
            xhigh = xlow = pshape[0];
            yhigh = ylow = pshape[1];
            for (int i = 0; i < pshape.Count-2; i+=2)
            {
                
                if(pshape[i] > xhigh)
                {
                    xhigh = pshape[i];
                }
                if (pshape[i] < xlow)
                {
                    xlow = pshape[i];
                }
                if (pshape[i+1] > yhigh)
                {
                    yhigh = pshape[i+1];
                }
                if (pshape[i+1] < ylow)
                {
                    ylow = pshape[i+1];
                }
                vecs.Add(new Vector3(pshape[i],0.2f,pshape[i+1]));
            }
            vecs.Add(new Vector3((xhigh+xlow)/2.0f, 0.2f, (yhigh + ylow) / 2.0f));

            mesh.vertices = vecs.ToArray();

            int[] tris = new int[(vecs.Count-1) * 3];
            int index = 0;
            for (int j = 0; j < tris.Length; j+=3)
            {
                tris[j] = index;
                tris[j + 1] = index + 1;
                tris[j + 2] = vecs.Count - 1;
                index++;
            }
            mesh.triangles = tris;

            // Build Normals
            Vector3[] norms = new Vector3[vecs.Count];
            for (int k = 0; k < norms.Length; k++)
            {
                norms[k] = -Vector3.up;
            }
            mesh.normals = norms;

            chunk.AddComponent<MeshFilter>().mesh = mesh;
            chunk.transform.parent = Structures_GO.transform;
        }
    }
}
