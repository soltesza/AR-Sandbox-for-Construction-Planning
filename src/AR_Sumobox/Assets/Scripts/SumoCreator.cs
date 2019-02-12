using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEditor;

public struct ProjectionData
{
    public string offset;
    public string originalBounds;
    public string projectedBounds;
}

public struct Lane
{
    public string id;
    public string index;
    public string speed;
    public string length;
    public string width;
    public string allow;
    public string disallow;
    public string shape;
}

public struct Edge
{
    public string id;
    public string from;
    public string to;
    public string name;
    public string shape;
    public string type;
    public List<Lane> lanes;
}

public struct Junction
{
    public string id;
    public string name;
    public string type;
    public string x;
    public string y;
    public string incomingLanes;
    public string internalLanes;
    public string shape;
}

public class SumoCreator : MonoBehaviour
{

    // A handle to the Terrain Plane and Network parent GameObject
    public GameObject Terrain_Plane;
    public GameObject Network_Parent;
    public ProjectionData Projection_Data;
    public List<Junction> Junctions;
    public List<Edge> Edges;

    private void Start()
    {
        Network_Parent = new GameObject();
    }

    // Gets the bounds of the current network as a pair of points in 2-Space
    private ProjectionData GetProjectionData(XmlDocument xml)
    {
        XmlNode location = xml.DocumentElement.SelectSingleNode("location");
        ProjectionData ret = new ProjectionData()
        {
            offset = location.Attributes.GetNamedItem("netOffset").Value,
            originalBounds = location.Attributes.GetNamedItem("origBoundary").Value,
            projectedBounds = location.Attributes.GetNamedItem("convBoundary").Value
        }; 
        return ret;
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
    private void BuildTerrain(ProjectionData PD)
    {
        List<float> bp = ShapeStringToFloatList(PD.projectedBounds);
        float x = (Math.Abs(bp[2] - bp[0]) + 100.0f) * 0.15f;
        float y = 1.0f;
        float z = (Math.Abs(bp[3] - bp[1]) + 100.0f) * 0.15f;
        Vector3 scale = new Vector3(x, y, z);
        GameObject terrain = Instantiate(Terrain_Plane, new Vector3(x/2.0f, 0.0f, y/2.0f), Quaternion.identity);
        terrain.transform.localScale = scale;
        terrain.SetActive(true);
        GameObject.Find("Main_Camera").transform.SetPositionAndRotation(new Vector3(x / 2.0f, 100.0f, y / 2.0f), Quaternion.Euler(new Vector3(90.0f,0.0f,0.0f)));
    }

    // Builds Road Junctions
    private void BuildJunction(Junction J)
    {
        // String points to floats
        if (J.shape == null || J.type == "internal")
        {
            return;
        }
        List<float> fshape = ShapeStringToFloatList(J.shape);
        int numverts = fshape.Count() / 2;

        // Center of junction
        float xjunc = float.Parse(J.x, CultureInfo.InvariantCulture.NumberFormat);
        float yjunc = float.Parse(J.y, CultureInfo.InvariantCulture.NumberFormat);
        Vector3 centerpoint = new Vector3(xjunc, yjunc, 0.2f);

        if (numverts > 5)
        {
            // Get Meshfilter and create a new mesh
            GameObject chunk = new GameObject();
            chunk.AddComponent<MeshRenderer>();
            Material m = chunk.GetComponent<MeshRenderer>().material;
            m.color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            m.SetFloat("Smoothness", 1.0f);
            Mesh mesh = new Mesh();

            // Build Vertices
            Vector3[] verts = new Vector3[numverts + 1];
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
            for (int k = 0; k < verts.Length - 1; k++)
            {
                tris[trisindex] = triscounter;
                tris[trisindex + 1] = verts.Length - 1;
                tris[trisindex + 2] = triscounter + 1;
                triscounter++;
                trisindex += 3;
            }
            tris[tris.Length - 1] = 0;
            tris[tris.Length - 2] = verts.Length - 1;
            tris[tris.Length - 3] = verts.Length - 2;
            mesh.triangles = tris;

            // Build Normals
            Vector3[] norms = new Vector3[numverts + 1];
            for (int k = 0; k < norms.Length; k++)
            {
                norms[k] = Vector3.up;
            }
            mesh.normals = norms;

            chunk.AddComponent<MeshFilter>().mesh = mesh;
            chunk.isStatic = true;
            chunk.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
            chunk.transform.Translate(new Vector3(centerpoint.x, 0.2f, centerpoint.y), Space.World);
            //chunk.transform.localScale = chunk.transform.localScale * 15.0f;
            chunk.transform.parent = Network_Parent.transform;
        }
    }

    // Builds Road Pieces
    private void BuildRoad(Edge edge)
    {
        
    }

    // Builds the network pieces by reading a Sumo network file
    // Reads through a network and saves all the network data to structs.
    // There are structs for projection parameters, edges, lanes and junctions.
    // After all data is read each struct will be looped through and its shape 
    // will be built with the according Build<PART>() function.
    // See BuildRoad(), BuildTerrain(), BuildJunction() above.
    private void BuildNetwork(string file)
    {
        // Open the file
        bool opened = true;
        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.Load(file);
        }
        catch (Exception e)
        {
            opened = false;
            UnityEngine.Debug.LogException(e);
        }
        if (opened)
        {
            // Get the network size information from the 'location' 
            // node and build the terrain
            Projection_Data = GetProjectionData(xmlDoc);
            BuildTerrain(Projection_Data);

            Junctions = new List<Junction>();
            Edges = new List<Edge>();
            XmlNodeList junctions = xmlDoc.DocumentElement.SelectNodes("junction");
            foreach (XmlNode junction in junctions)
            {
                Junction theJunction = new Junction();

                if (junction.Attributes["id"] != null)
                {
                    theJunction.id = junction.Attributes.GetNamedItem("id").Value;
                }
                if (junction.Attributes["name"] != null)
                {
                    theJunction.name = junction.Attributes.GetNamedItem("name").Value;
                }
                if (junction.Attributes["type"] != null)
                {
                    theJunction.type = junction.Attributes.GetNamedItem("type").Value;
                }
                else
                {
                    theJunction.type = "normal";
                }
                if (junction.Attributes["x"] != null)
                {
                    theJunction.x = junction.Attributes.GetNamedItem("x").Value;
                }
                if (junction.Attributes["y"] != null)
                {
                    theJunction.y = junction.Attributes.GetNamedItem("y").Value;
                }
                if (junction.Attributes["shape"] != null)
                {
                    theJunction.shape = junction.Attributes.GetNamedItem("shape").Value;
                }
                Junctions.Add(theJunction);
            }

            // Get all the edges in the network 
            XmlNodeList edges = xmlDoc.DocumentElement.SelectNodes("edge");
            foreach (XmlNode edge in edges)
            {
                Edge theEdge = new Edge();
                if (edge.Attributes["id"] != null)
                {
                    theEdge.id = edge.Attributes.GetNamedItem("id").Value;
                }
                if (edge.Attributes["name"] != null)
                {
                    theEdge.name = edge.Attributes.GetNamedItem("name").Value;
                }
                if (edge.Attributes["from"] != null)
                {
                    theEdge.from = edge.Attributes.GetNamedItem("from").Value;
                }
                if (edge.Attributes["to"] != null)
                {
                    theEdge.to = edge.Attributes.GetNamedItem("to").Value;
                }
                if (edge.Attributes["type"] != null)
                {
                    theEdge.type = edge.Attributes.GetNamedItem("type").Value;
                }
                if (edge.Attributes["shape"] != null)
                {
                    theEdge.shape = edge.Attributes.GetNamedItem("shape").Value;
                }

                theEdge.lanes = new List<Lane>();
                XmlNodeList lanes = edge.ChildNodes;
                foreach (XmlNode lane in lanes)
                {
                    Lane theLane = new Lane();
                    if (lane.Attributes["id"] != null)
                    {
                        theLane.id = lane.Attributes.GetNamedItem("id").Value;
                    }
                    if (lane.Attributes["width"] != null)
                    {
                        theLane.width = lane.Attributes.GetNamedItem("width").Value;
                    }
                    if (lane.Attributes["index"] != null)
                    {
                        theLane.index = lane.Attributes.GetNamedItem("index").Value;
                    }
                    if (lane.Attributes["speed"] != null)
                    {
                        theLane.speed = lane.Attributes.GetNamedItem("speed").Value;
                    }
                    if (lane.Attributes["length"] != null)
                    {
                        theLane.length = lane.Attributes.GetNamedItem("length").Value;
                    }
                    if (lane.Attributes["allow"] != null)
                    {
                        theLane.allow = lane.Attributes.GetNamedItem("allow").Value;
                    }
                    if (lane.Attributes["disallow"] != null)
                    {
                        theLane.disallow = lane.Attributes.GetNamedItem("disallow").Value;
                    }
                    if (lane.Attributes["shape"] != null)
                    {
                        theLane.shape = lane.Attributes.GetNamedItem("shape").Value;
                    }
                    theEdge.lanes.Add(theLane);
                }
                Edges.Add(theEdge);
            }
            // Build Roads here, Maybe move junctions here too
            foreach(Edge e in Edges)
            {
                BuildRoad(e);
            }
            foreach(Junction j in Junctions)
            {
                BuildJunction(j);
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
        string[] files = null;
        // Lets a user pick a generated network with a file selection prompt.
        try
        {
            string path = EditorUtility.OpenFolderPanel("Select a SUMO Network Folder.", "", "");
            files = Directory.GetFiles(path);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
        
        if(files != null)
        {
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
                        UnityEngine.Debug.Log("Unknown File Extention " + file.ToString());
                    }
                }
            }
        }
    }
}
