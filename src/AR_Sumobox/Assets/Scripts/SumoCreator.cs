using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class SumoCreator : MonoBehaviour
{

    public void CreateSumoConfigTemplate()
    {
        Process p = new Process();
        ProcessStartInfo si = new ProcessStartInfo();
        si.WindowStyle = ProcessWindowStyle.Hidden;
        si.FileName = "cmd.exe";
        si.Arguments = "/C sumo --save-template Assets/Generated/Config/temp.sumocfg --save-commented";
        p.StartInfo = si;
        p.Start();
        p.WaitForExit(5000);
    }

    public void GenerateOsmNetwork()
    {
        try
        {
            Process p = new Process();
            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "cmd.exe";
            si.Arguments = "/C osmWebWizard.py";
            si.UseShellExecute = false;
            si.RedirectStandardOutput = true;
            si.CreateNoWindow = true;
            p.StartInfo = si;
            p.Start();
            while (!p.StandardOutput.EndOfStream)
            {
                string last = p.StandardOutput.ReadLine();
                UnityEngine.Debug.Log(last);
                if (last.Contains("osm.sumocfg"))
                {
                    if(!p.HasExited)
                    {
                        p.Kill();
                    }
                    break;
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }
    }  
}
