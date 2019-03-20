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

	private Vector3 tolerance;
	private float timeAtPosition;
    private int curTriggerPositionIndex;
	private bool eventTriggered;
	private TrackableBehaviour.Status trackingStatus;
    public List<Bounds> triggerBounds = new List<Bounds>();

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
	}

    private bool WithinBoundsWithTolerance(Vector3 point, Bounds bounds, float tol)
    {
        Vector3 boundsVec = bounds.max - bounds.min;
        Vector3 pointToMin = bounds.min - point;
        float c = Vector3.Dot(boundsVec, pointToMin);
        double distance;

        // The closest point in the bounds is min
        if (c > 0.0f)
        {
            distance = Math.Sqrt(Math.Abs(Vector3.Dot(pointToMin, pointToMin)));
            return distance <= tol;
        }

        Vector3 pointToMax = point - bounds.max;

        // The closest point in the bounds is max
        if (Vector3.Dot(boundsVec, pointToMax) > 0.0f)
        {
            distance = Math.Sqrt(Math.Abs(Vector3.Dot(pointToMax, pointToMax)));
            return distance <= tol;
        }

        // The closest point in the bounds is between min and max
        Vector3 e = pointToMin - boundsVec * (c / Vector3.Dot(boundsVec, boundsVec));
        distance = Math.Sqrt(Math.Abs(Vector3.Dot(boundsVec, boundsVec)));

        return distance <= tol;
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
        for (int i = 0; i < triggerBounds.Count; i++)
        {
            Vector3 relativePos = mainCameraTransform.transform.InverseTransformDirection(triggerBounds[i].center - mainCameraTransform.transform.position);
            triggerBounds[i] = new Bounds(relativePos, triggerBounds[i].size);
        }
    }
    
	void Update()
	{
		if (trackingStatus != TrackableBehaviour.Status.NO_POSE) // Make sure the marker is being tracked
        {
            //SetTriggerAreasRelativeToCamera();

            Vector3 curPos = this.gameObject.transform.position;// Find the index of the trigger position that the marker is at
            int newTriggerPositionIndex = triggerBounds.FindIndex(b => b.size.magnitude > Mathf.Epsilon ? WithinBoundsWithTolerance(curPos, b, tolerance.x) : SameWithinTolerance(curPos, b.center, tolerance));

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
                    Debug.Log($"{this.name}: Event invoked.");
                    atPositionEvent.Invoke(curTriggerPositionIndex);
                    eventTriggered = true;
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
            triggerBounds.Add(new Bounds(triggerPosition, new Vector3(0, 0, 0)));
    }

    public void AddTriggerArea(Bounds triggerArea)
    {
        if (triggerArea != null)
            triggerBounds.Add(triggerArea);
    }

    public void AddTriggerAreas(IEnumerable<Vector3> triggerPositions)
    {
        if (triggerPositions != null)
            triggerBounds.AddRange(triggerPositions.Select(p => new Bounds(p, new Vector3(0, 0, 0))));
    }

    public void AddTriggerAreas(IEnumerable<Bounds> triggerAreas)
    {
        if (triggerAreas != null)
            triggerBounds.AddRange(triggerAreas);
    }
}