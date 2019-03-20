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

    private List<GameObject> lanes;
    private MarkerAction markerAction;
    private bool triggerAreasSet;

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

        triggerAreasSet = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!triggerAreasSet && lanesParentObject.transform.childCount > 0)
        {
            //lanes = new List<GameObject>(lanesParentObject.transform.childCount);

            //foreach (Transform child in lanesParentObject.transform)
            //{
            //    lanes.Add(child.gameObject);
            //}

            //markerAction.AddTriggerAreas(lanes.Select(l => l.GetComponent<LineRenderer>().bounds));

            //triggerAreasSet = true;

            //REMOVE ME
            lanes = new List<GameObject>(2);
            foreach (Transform child in lanesParentObject.transform)
            {
                if (child.gameObject.name == "-5248257#3_0" || child.gameObject.name == "-286524716_0")
                    lanes.Add(child.gameObject);
            }
        }

        //Bounds? test = lanes?.SingleOrDefault(l => l.name == "-315935169_0")?.GetComponent<LineRenderer>().bounds;
        
    }

    public void SetWorkZone(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex < lanes.Count)
        {
            lanes[laneIndex].GetComponent<Renderer>().material = workZoneMaterial;
        }
    }
}
