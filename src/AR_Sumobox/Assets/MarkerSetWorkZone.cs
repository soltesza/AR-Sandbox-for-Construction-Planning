using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Allows a marker to set a lane as a work zone
/// </summary>
public class MarkerSetWorkZone : MonoBehaviour
{
    public GameObject lanesParentObject;
    public Material workZoneMaterial;
    public TraciController traciController;

    private List<GameObject> roads;
    private MarkerAction markerAction;
    private bool triggerAreasSet;
    private Edge edge;

    // Start is called before the first frame update
    void Start()
    {
        markerAction = gameObject.GetComponent<MarkerAction>();
        if (markerAction == null)
        {
            throw new Exception("No Marker Action component found.");
        }

        if (lanesParentObject == null)
        {
            lanesParentObject = GameObject.Find("Edges");
        }

        if (traciController == null)
        {
            traciController = FindObjectOfType<TraciController>();
        }

        edge = lanesParentObject.GetComponent<Edge>();
        triggerAreasSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!triggerAreasSet && lanesParentObject.transform.childCount > 0)
        {
            roads = new List<GameObject>(lanesParentObject.transform.childCount);

            foreach (Transform child in lanesParentObject.transform)
            {
                if (edge.RoadList.Any(r => r.Id == child.name))
                    roads.Add(child.gameObject);
            }

            markerAction.AddTriggerAreas(roads.Select(l => l.GetComponent<LineRenderer>().bounds));

            triggerAreasSet = true;
        }
        
    }

    public void SetWorkZone(int roadIndex)
    {
        traciController.SetWorkZoneEntireRoad(roads[roadIndex]);

        if (roadIndex >= 0 && roadIndex < roads.Count)
        {
            foreach (Lane lane in edge.RoadList.Single(r => r.Id == roads[roadIndex].name).Lanes)
            {
                GameObject.Find(lane.Id).GetComponent<Renderer>().material = workZoneMaterial;
                Debug.Log($"Created work zone on lane {lane.Id}");
            }
        }
    }
}
