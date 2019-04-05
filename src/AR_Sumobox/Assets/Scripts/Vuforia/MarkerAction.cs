using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;
using System.Linq;
using System;

[Serializable]
public class IntEvent : UnityEvent<int> { }

/// <summary>
/// Used to provide functionality to a marker.
/// This script should only be attached to an Image Target.
/// </summary>
public class MarkerAction : MonoBehaviour, ITrackableEventHandler
{
	public float xYTolerance, zTolerance; // How close the marker must be to (x, y, z) to trigger the At Position event

	public IntEvent atPositionEvent;
	public float timeForTrigger; // Seconds
    public Transform mainCameraTransform;
    public float ARCameraHeight = 4350f;
    public MarkerManager markerManager;

    private Vector3 tolerance;
	private float timeAtPosition;
    private int curTriggerPositionIndex;
	private bool eventTriggered;
	private TrackableBehaviour.Status trackingStatus;
    public List<Bounds> triggerBounds = new List<Bounds>();
    private List<Vector3> triggerAbsolutePositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
	{
		tolerance = new Vector3(xYTolerance, xYTolerance, zTolerance);
		GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);
		eventTriggered = false;
        curTriggerPositionIndex = -1;

        if (mainCameraTransform == null)
        {
            mainCameraTransform = (GameObject.Find("Main Camera") ?? GameObject.Find("Main_Camera"))?.transform;
        }

        if (markerManager == null)
        {
            markerManager = FindObjectOfType<MarkerManager>();
        }
    }

    private float DistanceFromPointToLineSegment2D(Vector2 point, Bounds line)
    {
        Vector2 lineEnd1 = line.min;
        Vector2 lineEnd2 = line.max;

        // Find the projection from the point to the line
        float t = Vector2.Dot(point - lineEnd1, lineEnd2 - lineEnd1);
        t /= point.magnitude * point.magnitude;
        t = Mathf.Clamp(t, 0f, 1f);
        Vector2 proj = lineEnd1 + t * (lineEnd2 - lineEnd1);

        return Vector2.Distance(point, proj);
    }

    private bool WithinBoundsWithTolerance(Vector3 point, Bounds bounds, Vector3 tol)
    {
        float distanceIn2D = DistanceFromPointToLineSegment2D(point, bounds);
        return distanceIn2D <= tol.x && distanceIn2D <= tol.y && Mathf.Abs(point.z - bounds.center.z) <= tol.z;
    }

	private bool SameWithinTolerance(Vector3 v1, Vector3 v2, Vector3 tol)
	{
		bool sameX = (v1.x + tol.x >= v2.x) && (v1.x - tol.x <= v2.x);
		bool sameY = (v1.y + tol.y >= v2.y) && (v1.y - tol.y <= v2.y);
		bool sameZ = (v1.z + tol.z >= v2.z) && (v1.z - tol.z <= v2.z);

		return sameX && sameY && sameZ;
	}

    private void SetTriggerAreasRelativeToCamera()
    {
        float cameraHeight = mainCameraTransform.position.y;

        if (triggerAbsolutePositions.Count > 0)
        {
            cameraHeight -= triggerAbsolutePositions[0].y;
        }

        for (int i = 0; i < triggerBounds.Count; i++)
        {
            // First, set the position relative to the Main Camera
            Vector3 relativePos = mainCameraTransform.transform.InverseTransformDirection(triggerAbsolutePositions[i] - mainCameraTransform.transform.position);

            if (markerManager.rotateCamera)
            {
                // The webcam looks at the scene upside-down, so rotate the position 180 degrees
                relativePos = Vector3.Scale(relativePos, new Vector3(-1, -1, 1));
            }

            // Adjust proportions
            relativePos *= (ARCameraHeight / cameraHeight); 

            triggerBounds[i] = new Bounds(relativePos, triggerBounds[i].size);
        }
    }
    
	void Update()
	{
		if (trackingStatus != TrackableBehaviour.Status.NO_POSE) // Make sure the marker is being tracked
        {
            SetTriggerAreasRelativeToCamera();

            Vector3 curPos = this.gameObject.transform.position;
            
            // Find the index of the trigger position that the marker is at
            int newTriggerPositionIndex = -1;
            double minDist = double.MaxValue;
            for (int i = 0; i < triggerBounds.Count; i++)
            {
                double distance = triggerBounds[i].size.magnitude > Mathf.Epsilon ? DistanceFromPointToLineSegment2D(curPos, triggerBounds[i]) : Vector3.Distance(curPos, triggerBounds[i].center);
                if (distance < minDist)
                {
                    minDist = distance;

                    if (distance <= tolerance.x && distance <= tolerance.y && curPos.z - triggerBounds[i].center.z <= tolerance.z)
                    {
                        newTriggerPositionIndex = i;
                    }
                }
            }
            

            if (newTriggerPositionIndex == -1) // The marker is not at a trigger position
            {
                curTriggerPositionIndex = -1;
                eventTriggered = false;
            }
            else if (newTriggerPositionIndex == curTriggerPositionIndex) // The marker is still at the same trigger position
            {
                timeAtPosition += Time.deltaTime;

                if (!eventTriggered && timeAtPosition >= timeForTrigger)
                {
                    eventTriggered = true;
                    Debug.Log($"{this.name}: Event invoked.");
                    atPositionEvent.Invoke(curTriggerPositionIndex);
                }
            }
            else // The marker is at a new trigger position
            {
                curTriggerPositionIndex = newTriggerPositionIndex;
                timeAtPosition = 0.0f;
                eventTriggered = false;
            }
		}
	}

	public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
	{
		trackingStatus = newStatus;
		if (newStatus == TrackableBehaviour.Status.NO_POSE && previousStatus != TrackableBehaviour.Status.NO_POSE) // Just stopped tracking
		{
			timeAtPosition = 0.0f;
		}
	}
	
    public void AddTriggerArea(Vector3 triggerPosition)
    {
        if (triggerPosition != null)
        {
            triggerBounds.Add(new Bounds(triggerPosition, new Vector3(0, 0, 0)));
            triggerAbsolutePositions.Add(triggerPosition);
        }
    }

    public void AddTriggerArea(Bounds triggerArea)
    {
        if (triggerArea != null)
        {
            triggerBounds.Add(triggerArea);
            triggerAbsolutePositions.Add(triggerArea.center);
        }
    }

    public void AddTriggerAreas(IEnumerable<Vector3> triggerPositions)
    {
        if (triggerPositions != null)
        {
            triggerBounds.AddRange(triggerPositions.Select(p => new Bounds(p, new Vector3(0, 0, 0))));
            triggerAbsolutePositions.AddRange(triggerPositions);
        }
    }

    public void AddTriggerAreas(IEnumerable<Bounds> triggerAreas)
    {
        if (triggerAreas != null)
        {
            triggerBounds.AddRange(triggerAreas);
            triggerAbsolutePositions.AddRange(triggerAreas.Select(b => b.center));
        }
    }
}