using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEditor;

public class SumoCreator : MonoBehaviour
{
    public GameObject Terrain_Plane;

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

    // Builds the network pieces
    private void BuildNetwork(string file)
    {
        // Open the file
        XmlDocument xmlDoc = OpenXML(file);
        if (xmlDoc != null)
        {
            // Get the network size information from the 'location' node
            List<float> boundaryPoints = GetNetworkBounds(xmlDoc);
            BuildTerrain(boundaryPoints);

            // Get all the edges in the network 
            XmlNodeList edges = xmlDoc.DocumentElement.SelectNodes("edge");
            foreach (XmlNode edge in edges)
            {
                // Skip internal nodes but get the children of the rest
                if (edge.Attributes.GetNamedItem("function") != null && edge.Attributes.GetNamedItem("function").Value != "internal")
                {
                    // Children are lanes, get shape, width and length info if it exists
                    XmlNodeList lanes = edge.ChildNodes;
                    foreach (XmlNode lane in lanes)
                    {
                        string length, width, shape = null;
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
                            shape = lane.Attributes.GetNamedItem("shape").Value;
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
