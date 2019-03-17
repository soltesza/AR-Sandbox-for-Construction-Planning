using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEditor;

public class Junction : MonoBehaviour
{
    private GameObject Junctions_GO;

    public string Id;
    public string Name;
    public string Type;
    public string X;
    public string Y;
    public string IncomingLanes;
    public string InternalLanes;
    public string Shape;

    // Start is called before the first frame update
    void Start()
    {
        Junctions_GO = GameObject.Find("Junctions");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClearData()
    {
        Id = Name = Type = X = Y = IncomingLanes = InternalLanes = Shape = null;
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

    // Builds Road Junctions
    public void BuildJunction()
    {
        // String points to floats
        if (Shape == null || Type == "internal")
        {
            return;
        }
        List<float> fshape = ShapeStringToFloatList(Shape);
        int numverts = fshape.Count() / 2;

        // Center of junction
        float xjunc = float.Parse(X, CultureInfo.InvariantCulture.NumberFormat);
        float yjunc = float.Parse(Y, CultureInfo.InvariantCulture.NumberFormat);
        Vector3 centerpoint = new Vector3(xjunc, yjunc, 0.2f);

        if (numverts > 5)
        {
            // Get Meshfilter and create a new mesh
            GameObject chunk = new GameObject();
            if (Name != null)
            {
                chunk.name = Name;
            }
            else
            {
                chunk.name = Id;
            }
            
            chunk.AddComponent<MeshRenderer>();
            Material m = Resources.Load("Materials/Asphault_Material", typeof(Material)) as Material;
            chunk.GetComponent<MeshRenderer>().material = m;
            Mesh mesh = new Mesh();

            // Build Vertices
            Vector3[] verts = new Vector3[numverts + 1];
            int vc = 0;
            for (int i = 0; i < fshape.Count(); i = i + 2)
            {
                float fac1, fac2;
                if (fshape[i] < 0)
                {
                    fac1 = 1.9f;
                }
                else
                {
                    fac1 = -1.9f;
                }
                if (fshape[i+1] < 0)
                {
                    fac2 = 1.9f;
                }
                else
                {
                    fac2 = -1.9f;
                }

                verts[vc] = new Vector3(fshape[i] - fac1, 0.21f, fshape[i + 1] - fac2);
                vc++;
            }
            verts[verts.Length - 1] = new Vector3(centerpoint.x, 0.21f, centerpoint.y);
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
            for (int k = 0; k < norms.Length; k++)
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


}

