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
	public IntEvent atPositionEvent; 
	public float timeForTrigger; // (in seconds)
    public float xYTolerance, zTolerance;

    #region These values will be set by the Marker Manager
    [HideInInspector]
    public Transform mainCameraTransform;
    [HideInInspector]
    public float ARCameraHeight;
    [HideInInspector]
    public bool rotateCamera;
    #endregion

    private Vector3 tolerance;
	private float timeAtPosition;
    private int curTriggerPositionIndex;
	private bool eventTriggered;
	private TrackableBehaviour.Status trackingStatus;
    private List<Bounds> triggerBounds = new List<Bounds>(); // The zones that will cause the event to be invoked
    private List<Vector3> triggerAbsolutePositions = new List<Vector3>(); // The world coordinates of the objects that form the trigger areas
    
    void Start()
	{
		tolerance = new Vector3(xYTolerance, xYTolerance, zTolerance);
		GetComponent<TrackableBehaviour>().RegisterTrackableEventHandler(this);
		eventTriggered = false;
        curTriggerPositionIndex = -1;
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

                // Invoke the event if the marker has been at this position long enough
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

    /// <summary>
    /// Finds the shortest distance from a 2D point to a 2D line segment
    /// </summary>
    /// <param name="point">The point</param>
    /// <param name="line">The line segment (note: the z component will be ignored)</param>
    /// <returns>Returns the minimum distance between the point and the line segment</returns>
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

    /// <summary>
    /// Determines if a point is within the x/y/z tolerance of a line segment
    /// </summary>
    /// <param name="point">The point</param>
    /// <param name="bounds">The line segment to measure against</param>
    /// <param name="tol">How far away in x, y, and z directions the point can be from the line to return true</param>
    /// <returns>Returns true if the point is close enought to the line to be within tolerance; returns false otherwise</returns>
    private bool WithinBoundsWithTolerance(Vector3 point, Bounds bounds, Vector3 tol)
    {
        float distanceIn2D = DistanceFromPointToLineSegment2D(point, bounds);
        return distanceIn2D <= tol.x && distanceIn2D <= tol.y && Mathf.Abs(point.z - bounds.center.z) <= tol.z;
    }

    /// <summary>
    /// Adjusts the trigger areas to approximately line up with the position of the scene being projected into the sandbox.
    /// TODO: If we're only adjusting the bounds centers, we may also need to adjust the marker lengths. This would also require triggerAbsolutePositions to be a list of Bounds instead of a list of Vector3s.
    /// </summary>
    private void SetTriggerAreasRelativeToCamera()
    {
        float cameraHeight = mainCameraTransform.position.y;

        // Get the camera height relative to the objects in the scene
        if (triggerAbsolutePositions.Count > 0)
        {
            cameraHeight -= triggerAbsolutePositions[0].y;
        }

        // Adjust the trigger areas
        for (int i = 0; i < triggerBounds.Count; i++)
        {
            // First, set the position relative to the Main Camera
            Vector3 relativePos = mainCameraTransform.transform.InverseTransformDirection(triggerAbsolutePositions[i] - mainCameraTransform.transform.position);

            if (rotateCamera)
            {
                // The webcam looks at the scene upside-down, so rotate the position 180 degrees
                relativePos = Vector3.Scale(relativePos, new Vector3(-1, -1, 1));
            }

            // Adjust proportions
            relativePos *= (ARCameraHeight / cameraHeight); 

            triggerBounds[i] = new Bounds(relativePos, triggerBounds[i].size);
        }
    }

    /// <summary>
    /// Invoked by Vuforia whenever the status of an Image Target changes.
    /// This lets us know whether or not the marker is being tracked.
    /// </summary>
    /// <param name="previousStatus">The previous status of the Image Target</param>
    /// <param name="newStatus">The current status of the Image Target</param>
	public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
	{
		trackingStatus = newStatus;
		if (newStatus == TrackableBehaviour.Status.NO_POSE && previousStatus != TrackableBehaviour.Status.NO_POSE) // Just stopped tracking
		{
			timeAtPosition = 0.0f;
		}
	}
	
    /// <summary>
    /// Adds a point to the list of areas that trigger the event.
    /// </summary>
    /// <param name="triggerPosition">The point to add as a trigger.</param>
    public void AddTriggerArea(Vector3 triggerPosition)
    {
        if (triggerPosition != null)
        {
            triggerBounds.Add(new Bounds(triggerPosition, new Vector3(0, 0, 0)));
            triggerAbsolutePositions.Add(triggerPosition);
        }
    }

    /// <summary>
    /// Adds a Bounds to the list of areas that trigger the event.
    /// </summary>
    /// <param name="triggerArea">The area to add as a trigger.</param>
    public void AddTriggerArea(Bounds triggerArea)
    {
        if (triggerArea != null)
        {
            triggerBounds.Add(triggerArea);
            triggerAbsolutePositions.Add(triggerArea.center);
        }
    }

    /// <summary>
    /// Adds multiple points to the list of areas that trigger the event.
    /// </summary>
    /// <param name="triggerPositions">The points to add as triggers.</param>
    public void AddTriggerAreas(IEnumerable<Vector3> triggerPositions)
    {
        if (triggerPositions != null)
        {
            triggerBounds.AddRange(triggerPositions.Select(p => new Bounds(p, new Vector3(0, 0, 0))));
            triggerAbsolutePositions.AddRange(triggerPositions);
        }
    }

    /// <summary>
    /// Adds multiple Bounds to the list of areas that trigger the event.
    /// </summary>
    /// <param name="triggerAreas">The areas to add as triggers.</param>
    public void AddTriggerAreas(IEnumerable<Bounds> triggerAreas)
    {
        if (triggerAreas != null)
        {
            triggerBounds.AddRange(triggerAreas);
            triggerAbsolutePositions.AddRange(triggerAreas.Select(b => b.center));
        }
    }
}