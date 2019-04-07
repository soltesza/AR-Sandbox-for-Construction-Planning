using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Allows a marker to set a lane as a work zone.
/// This script should only be attached to an Image Target with a MarkerAction component
/// </summary>
public class MarkerSetWorkZone : MonoBehaviour
{
    public GameObject lanesParentObject;
    public Material workZoneMaterial;
    public TraciController traciController;

    private List<GameObject> roads;
    private MarkerAction markerAction;
    private bool triggerAreasSet;
    private Edge edgeScript;

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

        edgeScript = lanesParentObject.GetComponent<Edge>();
        triggerAreasSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Wait until we have loaded a network
        if (!triggerAreasSet && lanesParentObject.transform.childCount > 0)
        {
            roads = new List<GameObject>(lanesParentObject.transform.childCount);

            // Populate the list of roads with the child objects of Edges
            foreach (Transform child in lanesParentObject.transform)
            {
                // Some of the children are lanes (not roads) - we only want the roads
                if (edgeScript.RoadList.Any(r => r.Id == child.name))
                    roads.Add(child.gameObject);
            }

            // Set the trigger actions to the road bounds
            markerAction.AddTriggerAreas(roads.Select(l => l.GetComponent<LineRenderer>().bounds));

            triggerAreasSet = true;
        }
        
    }

    /// <summary>
    /// Called by MarkerAction when the trigger event is invoked.
    /// Creates a work zone for a given lane.
    /// </summary>
    /// <param name="roadIndex">The list index for the road. This index should be the same between this script and MarkerAction.</param>
    public void SetWorkZone(int roadIndex)
    {
        traciController.SetWorkZoneEntireRoad(roads[roadIndex]);

        // Set the material for each lane in the road to the work zone material
        if (roadIndex >= 0 && roadIndex < roads.Count)
        {
            foreach (Lane lane in edgeScript.RoadList.Single(r => r.Id == roads[roadIndex].name).Lanes)
            {
                GameObject.Find(lane.Id).GetComponent<Renderer>().material = workZoneMaterial;
                Debug.Log($"Created work zone on lane {lane.Id}");
            }
        }
    }
}
