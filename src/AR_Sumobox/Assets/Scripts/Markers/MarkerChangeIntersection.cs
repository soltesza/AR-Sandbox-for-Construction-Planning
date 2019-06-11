using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MarkerChangeIntersection : MonoBehaviour
{
    public Material trafficLightMaterial;
    public Material stopSignMaterial;
    public GameObject junctionsParentObject;
    public TraciController traciController;

    private List<GameObject> junctions;
    private MarkerAction markerAction;
    private bool triggerAreasSet;
    private Junction junctionScript;
    private Dictionary<string, string> junctionAndTrafficLightIds;

    void Start()
    {
        markerAction = gameObject.GetComponent<MarkerAction>();
        if (markerAction == null)
        {
            throw new Exception("No Marker Action component found.");
        }

        if (junctionsParentObject == null)
        {
            junctionsParentObject = GameObject.Find("Junctions");
        }

        if (traciController == null)
        {
            traciController = FindObjectOfType<TraciController>();
        }

        junctionScript = junctionsParentObject.GetComponent<Junction>();
        triggerAreasSet = false;

        junctionAndTrafficLightIds = new Dictionary<string, string>();
    }
    
    void Update()
    {
        // Wait until we have loaded a network
        if (!triggerAreasSet && junctionsParentObject.transform.childCount > 0)
        {
            junctions = new List<GameObject>(junctionsParentObject.transform.childCount);

            // Populate the list of roads with the child objects of Junctions
            foreach (Transform child in junctionsParentObject.transform)
            {
                // TODO: Do we want to filter the juctions at all? We may want to take out dead ends, for example
                junctions.Add(child.gameObject);
            }

            List<string[]> juncIncomingLaneIds = junctionScript.Junction_List.Select(j => j.IncomingLanes.Split(' ')).ToList();
            juncIncomingLaneIds.ForEach(j => Array.Sort(j));
            foreach (string trafficLightId in traciController.Client.TrafficLight.GetIdList().Content)
            {
                var trafficLight = traciController.Client.TrafficLight;
                List<List<string>> links = trafficLight.GetControlledLinks(trafficLightId).Content.Links;
                List<string> tlIncomingLaneList = new List<string>();
                foreach (var link in links)
                {
                    tlIncomingLaneList.Add(link[0]);
                }
                //string[] tlIncomingLaneIds = traciController.Client.TrafficLight.GetControlledLinks(trafficLightId).Content.Links.Select(l => l[0]).ToArray();
                string[] tlIncomingLaneIds = tlIncomingLaneList.ToArray();
                Array.Sort(tlIncomingLaneIds);
                Debug.Log(tlIncomingLaneIds);
                Debug.Log(tlIncomingLaneIds.Length);
                int junctionIdx = juncIncomingLaneIds.FindIndex(j => j.Any(l => tlIncomingLaneIds.Contains(l)));

                if (junctionIdx >= 0)
                    junctionAndTrafficLightIds.Add(junctionScript.Junction_List[junctionIdx].Id, trafficLightId);
            }

            // Set the trigger actions to the road bounds
            markerAction.AddTriggerAreas(junctions.Select(j => j.GetComponent<MeshRenderer>().bounds));

            triggerAreasSet = true;
        }
    }

    public void SetTrafficLightIntersection(int junctionIndex)
    {
        string trafficLightId = junctionAndTrafficLightIds[junctions[junctionIndex].name];
        traciController.SetTrafficLightJunction(trafficLightId);
        
        junctions[junctionIndex].GetComponent<Renderer>().material = trafficLightMaterial;
        Debug.Log($"Set junction {junctions[junctionIndex].name} to traffic light intersection.");
    }

    public void SetStopSignIntersection(int junctionIndex)
    {
        string trafficLightId = junctionAndTrafficLightIds[junctions[junctionIndex].name];
        traciController.SetStopSignJunction(trafficLightId);

        junctions[junctionIndex].GetComponent<Renderer>().material = stopSignMaterial;
        Debug.Log($"Set junction {junctions[junctionIndex].name} to stop sign intersection.");
    }
}
