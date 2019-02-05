using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class SumoCreator : MonoBehaviour
{
    // Let a user pick a generated network with a file selection prompt.
    private string[] OpenFileSelectionDialog()
    {
        string path = EditorUtility.OpenFolderPanel("Select a SUMO Network Folder.", "", "");
        string[] files = Directory.GetFiles(path);
        return files;
    }

    // Get Terrain size from Network file
    private string GetTerrainBounds(string file_ref)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(file_ref);
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("location");
            return node.Attributes.GetNamedItem("convBoundary").Value;
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e.ToString());
            return "";
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
                string networksize = GetTerrainBounds(file);
                string[] pointdata = networksize.Split(',');
                GameObject theterrain = GameObject.Find("Terrain");
                TerrainData td = theterrain.GetComponent<TerrainData>();
                float w = float.Parse(pointdata[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) - float.Parse(pointdata[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                float h = float.Parse(pointdata[3], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) - float.Parse(pointdata[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                td.size = new Vector3(w,h,10.0f);
                UnityEngine.Debug.Log(w.ToString()+h.ToString());
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
