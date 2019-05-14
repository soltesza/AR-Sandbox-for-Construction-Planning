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
    // Start is called before the first frame update
    void Start()
    {
        Traci_GO = GameObject.Find("Traci_Controller");
        TL_List = new List<Traffic_Light>();
    }

    public void Get_Traffic_Lights()
    {
        TraCIClient the_client = Traci_GO.GetComponent<TraciController>().Client;

        if (the_client != null)
        {
            List<string> tl_ids = the_client.TrafficLight.GetIdList().Content;
            foreach (string id in tl_ids)
            {
                Traffic_Light tl = new Traffic_Light();
                tl.Id = id;
                tl.ControlledLanes = the_client.TrafficLight.GetControlledLanes(id).Content;
                tl.Program = the_client.TrafficLight.GetCurrentProgram(id).Content;
                tl.PhaseDuration = (float)the_client.TrafficLight.GetPhaseDuration(id).Content;
                TL_List.Add(tl);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
