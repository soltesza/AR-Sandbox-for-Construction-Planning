using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class SumoCreator : MonoBehaviour
{

    public void GenerateOsmNetwork()
    {
        try
        {
            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "cmd.exe";
            si.Arguments = "/C osmWebWizard.py --remote --address=localhost --port=80";
            p.StartInfo = si;
            p.Start();

            Process p2 = new Process();
            ProcessStartInfo si2 = new ProcessStartInfo();
            si2.FileName = "cmd.exe";
            si2.Arguments = "/C C:/Sumo/tools/webWizard/index.html";
            p2.StartInfo = si2;
            p2.Start();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }  
}
