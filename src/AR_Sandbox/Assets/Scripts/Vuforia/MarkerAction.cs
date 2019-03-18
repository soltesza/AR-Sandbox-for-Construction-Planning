using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;
using System.Linq;

/// <summary>
/// Used to provide functionality to a marker.
/// This script should only be attached to an Image Target.
/// </summary>
public class MarkerAction : MonoBehaviour, ITrackableEventHandler
{

	public Vector3[] triggerPositions; // The positions that will trigger the At Position event
	public float xYTolerance, zTolerance; // How close the marker must be to (x, y, z) to trigger the At Position event

	public UnityEvent atPositionEvent;
	public float timeForTrigger;

	private Vector3 tolerance;
	private float timeAtPosition;
    private int curTriggerPositionIndex;
	private bool eventTriggered;
	private TrackableBehaviour.Status trackingStatus;

	// Start is called before the first frame update
	void Start()
	{
		tolerance = new Vector3(xYTolerance, xYTolerance, zTolerance);
		GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);
		eventTriggered = false;
        curTriggerPositionIndex = -1;
	}

	private bool SameWithinTolerance(Vector3 v1, Vector3 v2, Vector3 tol)
	{
		bool sameX = (v1.x + tol.x >= v2.x) && (v1.x - tol.x <= v2.x);
		bool sameY = (v1.y + tol.y >= v2.y) && (v1.y - tol.y <= v2.y);
		bool sameZ = (v1.z + tol.z >= v2.z) && (v1.z - tol.z <= v2.z);

		return sameX && sameY && sameZ;
	}
    
	void Update()
	{
		if (trackingStatus != TrackableBehaviour.Status.NO_POSE) // Make sure the marker is being tracked
        {
            Vector3 curPos = this.gameObject.transform.position;// Find the index of the trigger position that the marker is at
            int newTriggerPositionIndex = triggerPositions.ToList().FindIndex(p => SameWithinTolerance(p, curPos, tolerance));

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
                    atPositionEvent.Invoke();
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
		
}