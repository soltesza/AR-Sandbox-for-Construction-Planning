using System;
using System.Collections.Generic;
using CodingConnected.TraCI.NET;
using System.Globalization;
using UnityEngine;

/// <summary>
/// A struct representing a Sumo Network TrafficLight.
/// </summary>
[Serializable]
public struct Traffic_Light
{
    public string Id { get; set; }
    public List<string> ControlledLanes { get; set; }
    public string Program { get; set; }
    public float PhaseDuration { get; set; }
}

public class TrafficLight : MonoBehaviour
{
    private GameObject Traci_GO;
    public List<Traffic_Light> TL_List;
    // traffic light programs
    private string[] program_def;
    
    // Start is called before the first frame update
    void Start()
    {
        Traci_GO = GameObject.Find("Traci_Controller");
        TL_List = new List<Traffic_Light>();
        program_def = new string[5];
        program_def[0] = "GGggrrrrGGggrrrr";
        program_def[1] = "yyggrrrryyggrrrr";
        program_def[2] = "rrGGrrrrrrGGrrrr";
        program_def[3] = "rryyrrrrrryyrrrr";
        program_def[4] = "rrrrGGggrrrrGGgg";
    }

    public void Get_Traffic_Lights()
    {
        TraCIClient the_client = Traci_GO.GetComponent<TraciController>().Client;

        if (the_client != null)
        {
            List<string> tl_ids = the_client.TrafficLight.GetIdList().Content;
            if (tl_ids == null)
            {
                return;
            }
            foreach (string id in tl_ids)
            {
                Traffic_Light tl = new Traffic_Light();
                tl.Id = id;
                tl.ControlledLanes = the_client.TrafficLight.GetControlledLanes(id).Content;
                //List<List<string>> l = the_client.TrafficLight.GetControlledLinks(id).Content.Links;
                
                tl.Program = the_client.TrafficLight.GetCurrentProgram(id).Content;
                tl.PhaseDuration = (float)the_client.TrafficLight.GetPhaseDuration(id).Content;
                TL_List.Add(tl);
            }

            // A test traffic light complete program
            CodingConnected.TraCI.NET.Types.TrafficLightLogics tll = new CodingConnected.TraCI.NET.Types.TrafficLightLogics();
            tll.SubId = "1";
            tll.CurrentPhaseIndex = 0;
            tll.NumberOfPhases = 5;
            tll.TrafficLightPhases = new List<CodingConnected.TraCI.NET.Types.TrafficLightProgramPhase>();
            for (int i = 0; i < 5; i++)
            {
                CodingConnected.TraCI.NET.Types.TrafficLightProgramPhase tp = new CodingConnected.TraCI.NET.Types.TrafficLightProgramPhase();
                tp.Definition = program_def[i];
                tll.TrafficLightPhases.Add(tp);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
