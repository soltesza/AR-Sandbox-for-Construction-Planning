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
    public Material roadMaterial;
    public Material workZoneMaterial;
    public TraciController traciController;

    private List<GameObject> lanes;
    private MarkerAction markerAction;
    private bool triggerAreasSet;
    private Edge edgeScript;
    
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
    
    void Update()
    {
        // Wait until we have loaded a network
        if (!triggerAreasSet && lanesParentObject.transform.childCount > 0)
        {
            lanes = new List<GameObject>(lanesParentObject.transform.childCount);

            // Populate the list of roads with the child objects of Edges
            foreach (Transform child in lanesParentObject.transform)
            {
                // Some of the children are roads (not lanes) - we only want the lanes
                if (edgeScript.RoadList.Any(r => r.Lanes.Any(l => l.Id == child.name)))
                    lanes.Add(child.gameObject);
            }

            // Set the trigger actions to the road bounds
            markerAction.AddTriggerAreas(lanes.Select(l => l.GetComponent<LineRenderer>().bounds));

            triggerAreasSet = true;
        }
        
    }

    /// <summary>
    /// Called by MarkerAction when the event is triggered.
    /// Creates a work zone at the given road if it is not already set as a work zone.
    /// Removes a work zone from the given road if it is currently set as a work zone.
    /// </summary>
    /// <param name="laneIndex">The list index for the road. This index should be the same between this script and MarkerAction.</param>
    public void ToggleWorkZone(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex < lanes.Count)
        {
            Road road = edgeScript.RoadList.Single(r => r.Lanes.Any(l => l.Id == lanes[laneIndex].name));

            if (road.Lanes.Single(l => l.Id == lanes[laneIndex].name).ConstructionZone)
            {
                RemoveWorkZone(lanes[laneIndex], road.Id);
            }
            else
            {
                SetWorkZone(lanes[laneIndex], road.Id);
            }
        }
    }

    /// <summary>
    /// Creates a work zone for a given road.
    /// </summary>
    /// <param name="laneStruct">The Road struct holding the road's information</param>
    /// <param name="roadId">The road's game object</param>
    public void SetWorkZone(GameObject lane, string roadId)
    {
        traciController.SetWorkZoneOneLane(roadId, lane.name);

        // Set the material for each lane in the road to the work zone material
        lane.GetComponent<Renderer>().material = workZoneMaterial;
        Debug.Log($"Created work zone on lane {lane.name}");
    }

    /// <summary>
    /// Removes a work zone from a given road
    /// </summary>
    /// <param name="roadStruct">The Road struct holding the road's information</param>
    /// <param name="roadId">The road's game object</param>
    public void RemoveWorkZone(GameObject lane, string roadId)
    {
        traciController.RemoveWorkZoneOnLane(roadId, lane.name);

        // Set the material for each lane to its original material
        lane.GetComponent<Renderer>().material = roadMaterial;
        Debug.Log($"Removed work zone from lane {lane.name}");
    }
}
