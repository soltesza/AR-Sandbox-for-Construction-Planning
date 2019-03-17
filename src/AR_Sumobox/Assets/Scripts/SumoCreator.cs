using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using CodingConnected.TraCI.NET;

/// SumoCreator class is used for creating Open Street Map networks with SUMO's
/// OSM Web Wizard and reading SUMO generated files that describe a networks logic 
/// and layout. 
public class SumoCreator : MonoBehaviour
{
    /// ProjectionData Parent GameObject and script.
    private GameObject Projection_Data_GO;
    /// Junctions Parent GameObject and script.
    private GameObject Junctions_GO;
    /// Edges Parent GameObject and script.
    private GameObject Edges_GO;
    /// Structures Parent GameObject and script.
    private GameObject Structures_GO;
    /// TraciController Game object (Script)
    private TraciController TraciClient;

    /// Find all game objects at start.
    private void Start()
    {
        Projection_Data_GO = GameObject.Find("Projection_Data");
        Junctions_GO = GameObject.Find("Junctions");
        Edges_GO = GameObject.Find("Edges");
        Structures_GO = GameObject.Find("Structures");
        TraciClient = GameObject.FindObjectOfType(typeof(TraciController)) as TraciController;
    }

    /// Builds the network pieces by reading a Sumo network file.
    /// Reads through a network and saves all the network data to the handling class.
    /// There are classes for ProjectionData, Edge, and Junction.
    /// After all data is read in each class builds its own shapes. 
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
            // node and build the terrain.
            Projection_Data_GO.GetComponent<ProjectionData>().SetProjectionData(xmlDoc);
            Projection_Data_GO.GetComponent<ProjectionData>().BuildTerrain();

            // Get all the Junction/Intersection information from the 'junction' 
            // nodes and build the Junctions.
            XmlNodeList junctions = xmlDoc.DocumentElement.SelectNodes("junction");
            foreach (XmlNode junction in junctions)
            {
                if (junction.Attributes["id"] != null)
                {
                    Junctions_GO.GetComponent<Junction>().Id = junction.Attributes.GetNamedItem("id").Value;
                }
                if (junction.Attributes["name"] != null)
                {
                    Junctions_GO.GetComponent<Junction>().Name = junction.Attributes.GetNamedItem("name").Value;
                }
                if (junction.Attributes["type"] != null)
                {
                    Junctions_GO.GetComponent<Junction>().Type = junction.Attributes.GetNamedItem("type").Value;
                }
                else
                {
                    Junctions_GO.GetComponent<Junction>().Type = "normal";
                }
                if (junction.Attributes["x"] != null)
                {
                    Junctions_GO.GetComponent<Junction>().X = junction.Attributes.GetNamedItem("x").Value;
                }
                if (junction.Attributes["y"] != null)
                {
                    Junctions_GO.GetComponent<Junction>().Y = junction.Attributes.GetNamedItem("y").Value;
                }
                if (junction.Attributes["shape"] != null)
                {
                    Junctions_GO.GetComponent<Junction>().Shape = junction.Attributes.GetNamedItem("shape").Value;
                }
                Junctions_GO.GetComponent<Junction>().BuildJunction();
                Junctions_GO.GetComponent<Junction>().ClearData();
            }

            // Get all the Edge/Road information from the 'edge' nodes.
            // Then build the Roads.
            XmlNodeList edges = xmlDoc.DocumentElement.SelectNodes("edge");
            foreach (XmlNode edge in edges)
            {
                // Create the Road and add the Edges Attributes.
                // An Id will always be present but need to check the rest.
                Road newEdge = new Road();
                newEdge.Id = edge.Attributes.GetNamedItem("id").Value;
                if (edge.Attributes["name"] != null)
                {
                    newEdge.Name = edge.Attributes.GetNamedItem("name").Value;
                }
                if (edge.Attributes["from"] != null)
                {
                    newEdge.From = edge.Attributes.GetNamedItem("from").Value;
                }
                if (edge.Attributes["to"] != null)
                {
                    newEdge.To = edge.Attributes.GetNamedItem("to").Value;
                }
                if (edge.Attributes["type"] != null)
                {
                    newEdge.Type = edge.Attributes.GetNamedItem("type").Value;
                }
                if (edge.Attributes["shape"] != null)
                {
                    newEdge.Shape = edge.Attributes.GetNamedItem("shape").Value;
                }

                // Get all the Lanes that belong to the current Edge. 
                newEdge.Lanes = new List<Lane>();
                XmlNodeList lanes = edge.ChildNodes;
                foreach (XmlNode lane in lanes)
                {
                    // Create a new Lane and add the Lanes Attributes.
                    // Then save the Lane in the Road.Lanes list.
                    Lane theLane = new Lane();
                    if (lane.Attributes["id"] != null)
                    {
                        theLane.Id = lane.Attributes.GetNamedItem("id").Value;
                    }
                    if (lane.Attributes["width"] != null)
                    {
                        theLane.Width = lane.Attributes.GetNamedItem("width").Value;
                    }
                    if (lane.Attributes["index"] != null)
                    {
                        theLane.Index = lane.Attributes.GetNamedItem("index").Value;
                    }
                    if (lane.Attributes["speed"] != null)
                    {
                        theLane.Speed = lane.Attributes.GetNamedItem("speed").Value;
                    }
                    if (lane.Attributes["length"] != null)
                    {
                        theLane.Length = lane.Attributes.GetNamedItem("length").Value;
                    }
                    if (lane.Attributes["allow"] != null)
                    {
                        theLane.Allow = lane.Attributes.GetNamedItem("allow").Value;
                    }
                    if (lane.Attributes["disallow"] != null)
                    {
                        theLane.Disallow = lane.Attributes.GetNamedItem("disallow").Value;
                    }
                    if (lane.Attributes["shape"] != null)
                    {
                        theLane.Shape = lane.Attributes.GetNamedItem("shape").Value;
                    }
                    newEdge.Lanes.Add(theLane);
                }

                // Save negative and positive edges seperatly.
                // A negative edge always has a positive counterpart that 
                // makes an entire road section
                if (newEdge.Id[0] == '-')
                {
                    Edges_GO.GetComponent<Edge>().RoadList_Neg.Add(newEdge);
                }
                else
                {
                    Edges_GO.GetComponent<Edge>().RoadList_Pos.Add(newEdge);
                }
            }
            // Let the Edge script build all the Networks Roads/Edges.
            // This can be a very time consuming function given a large network.
            Edges_GO.GetComponent<Edge>().BuildEdges();
        }
    }

    /// Parse the XML file and procedurally build all buildings and landmarks
    private void BuildStructures(string file)
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
            XmlNodeList polys = xmlDoc.DocumentElement.SelectNodes("poly");
            foreach (XmlNode poly in polys)
            {
                Poly newpoly = new Poly();
                newpoly.Id = poly.Attributes.GetNamedItem("id").Value;
                if (poly.Attributes["color"] != null)
                {
                    newpoly.Color = poly.Attributes.GetNamedItem("color").Value;
                }
                if (poly.Attributes["type"] != null)
                {
                    newpoly.Type = poly.Attributes.GetNamedItem("type").Value;
                }
                if (poly.Attributes["shape"] != null)
                {
                    newpoly.Shape = poly.Attributes.GetNamedItem("shape").Value;
                }
                Structures_GO.GetComponent<Structure>().Polys.Add(newpoly);
            }
            //Structures_GO.GetComponent<Structure>().Build();
        }
    }

    /// Open Up OSMWebWizard and let the user build a real network.
    /// The user will save the new network to a zipfile when done.
    /// The processes remain open so the user can build multiple network at once.
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

    /// Goes through all network description files and build the network into Unity.
    /// Most files will be passed over at this point but there are some handles left 
    /// in case we decide we need access to them later.
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

        if (files != null)
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
                    BuildStructures(file);
                }
                // The view file.
                else if (file.EndsWith(".view.xml"))
                {
                    continue;
                }
                // The config file.
                else if (file.EndsWith(".sumocfg"))
                {
                    StartSumo(file);
                    continue;
                }
                else
                {
                    // Ignore all batch files but we should log unknowns.
                    if (!file.EndsWith(".bat"))
                    {
                        UnityEngine.Debug.Log("Unknown File Extention " + file.ToString());
                    }
                }
            }
        }
    }

    private void StartSumo(string ConfigFile)
    {
        try
        {
            TraciClient.Port = 80;
            TraciClient.HostName = "localhost";
            TraciClient.ConfigFile = ConfigFile;
            TraciClient.Invoke("ConnectToSumo", 5);             
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }
}
