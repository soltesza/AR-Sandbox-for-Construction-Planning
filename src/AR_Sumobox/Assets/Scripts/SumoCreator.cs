using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEditor;

public class SumoCreator : MonoBehaviour
{
    // A handle to the Terrain Plane
    public GameObject Terrain_Plane;
    public GameObject Network_Parent;

    void Start()
    {
        Network_Parent = new GameObject();
    }

    // Lets a user pick a generated network with a file selection prompt.
    private string[] OpenFileSelectionDialog()
    {
        try
        {
            string path = EditorUtility.OpenFolderPanel("Select a SUMO Network Folder.", "", "");
            string[] files = Directory.GetFiles(path);
            return files;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
            return null;
        }
    }

    // Opens and returns an XML document
    private XmlDocument OpenXML(string filename)
    {
        try
        {
            XmlDocument x = new XmlDocument();
            x.Load(filename);
            return x;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
            return null;
        }
    }

    // Gets the bounds of the current network as two pairs of points in 2-Space
    private List<float> GetNetworkBounds(XmlDocument xml)
    {
        try
        {
            XmlNode location = xml.DocumentElement.SelectSingleNode("location");
            string[] bounds = location.Attributes.GetNamedItem("convBoundary").Value.Split(',');
            List<string> boundarys = bounds.ToList();
            List<float> boundaryPoints = new List<float>();
            foreach (string b in boundarys)
            {
                boundaryPoints.Add(float.Parse(b, CultureInfo.InvariantCulture.NumberFormat));
            }
            return boundaryPoints;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
            return null;
        }  
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

    // Adds a Terrain_Plane prefab to the scene 100 units (meters) larger than the network.
    private void BuildTerrain(List<float> bp)
    {
        float x = Math.Abs(bp[2] - bp[0]) + 100.0f;
        float y = 1.0f;
        float z = Math.Abs(bp[3] - bp[1]) + 100.0f;
        Vector3 scale = new Vector3(x, y, z);
        GameObject terrain = Instantiate(Terrain_Plane, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        terrain.transform.localScale = scale;
        terrain.SetActive(true);
    }

    // Builds Road Junctions
    private void Build_Junction(string x, string y, string shape)
    {
        // String points to floats
        List<float> fshape = ShapeStringToFloatList(shape);
        int numverts = fshape.Count()/2;

        // Center of junction
        float xjunc = float.Parse(x, CultureInfo.InvariantCulture.NumberFormat);
        float yjunc = float.Parse(y, CultureInfo.InvariantCulture.NumberFormat);
        Vector3 centerpoint = new Vector3(xjunc, yjunc, 0.2f);

        if (numverts > 5)
        {
            // Get Meshfilter and create a new mesh
            GameObject chunk = new GameObject();
            chunk.AddComponent<MeshRenderer>();
            Material m = chunk.GetComponent<MeshRenderer>().material;
            m.color = new Vector4(0.5f,0.5f,0.5f,1.0f);
            m.SetFloat("Smoothness", 0.0f);
            Mesh mesh = new Mesh();

            // Build Vertices
            Vector3[] verts = new Vector3[numverts+1];
            int vc = 0;
            for (int i = 0; i < fshape.Count(); i = i + 2)
            {
                verts[vc] = new Vector3(fshape[i] - centerpoint.x, fshape[i + 1] - centerpoint.y, 0.2f);
                vc++;
            }
            verts[verts.Length - 1] = new Vector3(0.0f, 0.0f, 0.2f);
            mesh.vertices = verts;

            // Build Triangles
            int[] tris = new int[(verts.Length - 1) * 3];
            int triscounter = 0;
            int trisindex = 0;
            for(int k = 0; k < verts.Length - 1; k++)
            {
                tris[trisindex] = triscounter;
                tris[trisindex+1] = verts.Length - 1;
                tris[trisindex+2] = triscounter+1;
                triscounter++;
                trisindex += 3;
            }
            tris[tris.Length - 1] = 0;
            tris[tris.Length - 2] = verts.Length - 1;
            tris[tris.Length - 3] = verts.Length - 2;
            mesh.triangles = tris;

            // Build Normals
            Vector3[] norms = new Vector3[numverts+1];
            for (int k = 0; k < norms.Length; k++)
            {
                norms[k] = Vector3.up;
            }
            mesh.normals = norms;

            chunk.AddComponent<MeshFilter>().mesh = mesh;
            chunk.isStatic = false;
            chunk.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
            chunk.transform.Translate(new Vector3(centerpoint.x * 5.0f, centerpoint.z, centerpoint.y * 5.0f), Space.World);
            chunk.transform.localScale = chunk.transform.localScale * 5.0f;
            chunk.transform.parent = Network_Parent.transform;
        }
    }

    // Builds the network pieces
    private void BuildNetwork(string file)
    {
        // Open the file
        XmlDocument xmlDoc = OpenXML(file);
        if (xmlDoc != null)
        {
            // Get the network size information from the 'location' 
            // node and build the terrain
            List<float> boundaryPoints = GetNetworkBounds(xmlDoc);
            BuildTerrain(boundaryPoints);

            XmlNodeList junctions = xmlDoc.DocumentElement.SelectNodes("junction");
            foreach (XmlNode junction in junctions)
            {
                string x = null;
                string y = null;
                string jshape = null;
                if (junction.Attributes["x"] != null)
                {
                    x = junction.Attributes.GetNamedItem("x").Value;
                }
                if (junction.Attributes["y"] != null)
                {
                    y = junction.Attributes.GetNamedItem("y").Value;
                }
                if (junction.Attributes["shape"] != null)
                {
                    jshape = junction.Attributes.GetNamedItem("shape").Value;
                }
                if(x != null && y != null && jshape != null)
                {
                    Build_Junction(x, y, jshape);
                    //UnityEngine.Debug.Log(x + " " + y + " " + jshape);
                }
                
            }

            // Get all the edges in the network 
            XmlNodeList edges = xmlDoc.DocumentElement.SelectNodes("edge");
            foreach (XmlNode edge in edges)
            {
                // Skip internal edges but get the children of the rest
                if (edge.Attributes.GetNamedItem("function") != null && edge.Attributes.GetNamedItem("function").Value != "internal")
                {
                    // Children are lanes, get shape, width and length info if it exists
                    XmlNodeList lanes = edge.ChildNodes;
                    foreach (XmlNode lane in lanes)
                    {
                        string length = null;
                        string width = null; 
                        string eshape = null;
                        if (lane.Attributes["length"] != null)
                        {
                            length = lane.Attributes.GetNamedItem("length").Value;
                        }
                        if (lane.Attributes["width"] != null)
                        {
                            width = lane.Attributes.GetNamedItem("width").Value;
                        }
                        if (lane.Attributes["shape"] != null)
                        {
                            eshape = lane.Attributes.GetNamedItem("shape").Value;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }

    // Open Up OSMWebWizard and let the user build a real network.
    // The user will save the new network to a zipfile when done.
    // The processes remain open so the user can build multiple network at once.
    public void GenerateOsmNetwork()
    {
        try
        {
            // OSMWebWizard Server
            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/C osmWebWizard.py --remote --address=localhost --port=80"
            };
            p.StartInfo = si;
            p.Start();

            // The Javascript Client
            Process p2 = new Process();
            ProcessStartInfo si2 = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/C C:/Sumo/tools/webWizard/index.html"
            };
            p2.StartInfo = si2;
            p2.Start();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }

    // Go through all network description files and build the network into Unity.
    // Most files will be passed over at this point but there are some handles left 
    // in case we decide we need access to them later.
    public void LoadNetwork()
    {
        string[] files = OpenFileSelectionDialog();
        foreach (string file in files)
        {
            // The trips files.
            if (file.EndsWith(".trips.xml"))
            {
                continue;
            }
            // The routes files.
            else if (file.EndsWith(".rou.xml"))
            {
                continue;
            }
            // The network file.
            else if (file.EndsWith(".net.xml"))
            {
                BuildNetwork(file);
            }
            // The polygon file.
            else if (file.EndsWith(".poly.xml"))
            {
                continue;
            }
            // The view file.
            else if (file.EndsWith(".view.xml"))
            {
                continue;
            }
            // The config file.
            else if (file.EndsWith(".sumocfg"))
            {
                continue;
            }
            else
            {
                // Ignore all batch files
                if (!file.EndsWith(".bat"))
                {
                    string extmess = "Unknown File Extention " + file;
                    UnityEngine.Debug.Log(extmess);
                }
            }
        }
    }
}
