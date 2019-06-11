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
using System.Net;

/// <summary>
/// SumoCreator class is used for creating Open Street Map networks with SUMO's
/// OSM Web Wizard and reading SUMO generated files that describe a networks logic 
/// and layout.
/// </summary>
public class SumoCreator : MonoBehaviour
{
    /// <summary>
    /// ProjectionData parent GameObject and script.
    /// </summary>
    private GameObject Projection_Data_GO;
    /// <summary>
    /// Junctions parent GameObject and script.
    /// </summary>
    private GameObject Junctions_GO;
    /// <summary>
    /// Edges parent GameObject and script.
    /// </summary>
    private GameObject Edges_GO;
    /// <summary>
    /// Structures parent GameObject and script.
    /// </summary>
    private GameObject Structures_GO;
    /// <summary>
    /// TraciController Game object (Script)
    /// </summary>
    private GameObject Traci_GO;
    /// <summary>
    /// TrafficLight Game object (Script)
    /// </summary>
    private GameObject TrafficLight_GO;
    /// <summary>
    /// A handle to the main scene camera.
    /// </summary>
    private GameObject Main_Camera;

    /// <summary>
    /// The name of the current simulation configuration file.
    /// </summary>
    private string CFG_FILE = null;
    private string NET_FILE = null;
    private string POLY_FILE = null;
    /// <summary>
    /// Find all parent GameObjects at start.
    /// </summary>
    private void Start()
    {
        Projection_Data_GO = GameObject.Find("Projection_Data");
        Junctions_GO = GameObject.Find("Junctions");
        Edges_GO = GameObject.Find("Edges");
        Structures_GO = GameObject.Find("Structures");
        Traci_GO = GameObject.Find("Traci_Controller");
        Main_Camera = GameObject.Find("Main_Camera");
        TrafficLight_GO = GameObject.Find("TrafficLights");
    }

    /// <summary>
    /// Builds the network pieces by parsing a Sumo network file.
    /// Reads through a network and saves all the network data to the handling class.
    /// There are classes for ProjectionData, Edge, Junction and Structure.
    /// After all data is read in each class builds its own shapes.
    /// </summary>
    /// <param name="file">The name of the SUMO .net file to parse given as a string.</param>
    private void BuildNetwork(string file)
    {
        Main_Camera.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
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
            XmlNodeList junctions2 = xmlDoc.SelectNodes("junction");

            foreach (XmlNode junction in junctions)
            {
                Intersection theJunction = new Intersection();
                UnityEngine.Debug.Log(junctions.Count.ToString());
                if (junction.Attributes["id"] != null)
                {
                    theJunction.Id = junction.Attributes.GetNamedItem("id").Value;
                }
                if (junction.Attributes["name"] != null)
                {
                    theJunction.Name = junction.Attributes.GetNamedItem("name").Value;
                }
                if (junction.Attributes["type"] != null)
                {
                    theJunction.Type = junction.Attributes.GetNamedItem("type").Value;
                }
                else
                {
                    theJunction.Type = "normal";
                }
                if (junction.Attributes["x"] != null)
                {
                    theJunction.X = junction.Attributes.GetNamedItem("x").Value;
                }
                if (junction.Attributes["y"] != null)
                {
                    theJunction.Y = junction.Attributes.GetNamedItem("y").Value;
                }
                if (junction.Attributes["incLanes"] != null)
                {
                    theJunction.IncomingLanes = junction.Attributes.GetNamedItem("incLanes").Value;
                }
                if (junction.Attributes["shape"] != null)
                {
                    theJunction.Shape = junction.Attributes.GetNamedItem("shape").Value;
                }
                Junctions_GO.GetComponent<Junction>().Junction_List.Add(theJunction);
            }
            var ids = Junctions_GO.GetComponent<Junction>().Junction_List.Select(j => j.Id);
            var types = Junctions_GO.GetComponent<Junction>().Junction_List.Select(j => j.Type);
            Junctions_GO.GetComponent<Junction>().Build();

            // Get all the Edge/Road information from the 'edge' nodes.
            // Then build the Roads.
            XmlNodeList edges = xmlDoc.DocumentElement.SelectNodes("edge");
            foreach (XmlNode edge in edges)
            {
                // Create the Road and add the Edges Attributes.
                // An Id will always be present but need to check the rest.
                Road newEdge = new Road();
                newEdge.Built = false;
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
                if (edge.Attributes["function"] != null)
                {
                    newEdge.Function = edge.Attributes.GetNamedItem("function").Value;
                }
                else
                {
                    newEdge.Function = "normal";
                }

                // Get all the Lanes that belong to the current Edge. 
                newEdge.Lanes = new List<Lane>();
                XmlNodeList lanes = edge.ChildNodes;
                foreach (XmlNode lane in lanes)
                {
                    // Create a new Lane and add the Lanes Attributes.
                    // Then save the Lane in the Road.Lanes list.
                    Lane theLane = new Lane();
                    theLane.Built = false;
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
                        theLane.DefaultSpeed = theLane.Speed;
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
                Edges_GO.GetComponent<Edge>().RoadList.Add(newEdge);
            }
            // Let the Edge script build all the Networks Roads/Edges.
            // This can be a very time consuming function given a large network.
            Edges_GO.GetComponent<Edge>().BuildEdges();
        }
    }

    /// <summary>
    /// Parse the XML file and procedurally create all buildings and landmarks.
    /// </summary>
    /// <param name="file">The name of the SUMO .net file to parse given as a string.</param>
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
            Structures_GO.GetComponent<Structure>().Build();
        }
    }

    /// <summary>
    /// Open the OSMWebWizard to build a real world road network.
    /// The user will save the new network to a zipfile when done.
    /// The processes remain open so the user can build multiple network at once.
    /// </summary>
    public void GenerateOsmNetwork()
    {
        Main_Camera.GetComponentInChildren<Canvas>()?.gameObject?.SetActive(false);
        try
        {
            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo()
            {
                WorkingDirectory = "C:\\Sumo\\tools\\",
                FileName = "osmWebWizard.py"
            };
            p.StartInfo = si;
            p.Start();
           
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// Go through all network description files and build the network into Unity.
    /// Most files will be passed over but there are some handles left for upgrades.
    /// </summary>
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
                    NET_FILE = file;
                    
                }
                // The polygon file.
                else if (file.EndsWith(".poly.xml"))
                {
                    POLY_FILE = file;   
                }
                // The view file.
                else if (file.EndsWith(".view.xml"))
                {
                    continue;
                }
                // The config file.
                else if (file.EndsWith(".sumocfg"))
                {
                    CFG_FILE = file;
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
            //UnityEngine.Debug.Assert(CFG_FILE != null, "No .sumocfg file created, something may have gone wrong with the osmWebWizard.py. Try using Python 2.X with unicode support");
            if (CFG_FILE != null)
            {
                StartSumo(CFG_FILE);
            }
            if (NET_FILE != null)
            {
                BuildNetwork(NET_FILE);
            }
            if (POLY_FILE != null)
            {
                BuildStructures(POLY_FILE);
            }
        }
    }

    /// <summary>
    /// Starts Traci and Sumo to run traffic simulations
    /// </summary>
    /// <param name="ConfigFile"></param>
    private void StartSumo(string ConfigFile)
    {
        try
        {
            Traci_GO.GetComponent<TraciController>().Port = 80;
            Traci_GO.GetComponent<TraciController>().HostName = Dns.GetHostEntry("localhost").AddressList[1].ToString();
            Traci_GO.GetComponent<TraciController>().ConfigFile = ConfigFile;
            Traci_GO.GetComponent<TraciController>().Invoke("ConnectToSumo", 0.0f);             
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }
}
