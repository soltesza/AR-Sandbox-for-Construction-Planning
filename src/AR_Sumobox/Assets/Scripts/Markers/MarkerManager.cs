using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// Provides global management and functionality for markers.
/// There should only be one MarkerManager in the scene.
/// </summary>
public class MarkerManager : MonoBehaviour
{
    public bool rotateCamera;
    public Transform mainCameraTransform;
    public float ARCameraHeight = 4350f; // Determine this value by taking the z value of the marker when it is placed flat in the sandbox directly under the webcam
    public Text uiText;

    [HideInInspector]
    public List<MarkerAction> markerActionScripts;

    private List<ImageTargetBehaviour> markers;
    private List<TrackableBehaviour.Status> statuses;
    private int numTrackedMarkers;
    
    void Start()
    {
        markers = FindObjectsOfType<ImageTargetBehaviour>().ToList();
        markerActionScripts = FindObjectsOfType<MarkerAction>().ToList();
        statuses = new List<TrackableBehaviour.Status>(markers.Count);
        numTrackedMarkers = 0;

        if (mainCameraTransform == null)
        {
            mainCameraTransform = (GameObject.Find("Main Camera") ?? GameObject.Find("Main_Camera"))?.transform;
        }
    }
    
    void Update()
    {
        statuses = markers.Select(m => m.CurrentStatus).ToList();
        numTrackedMarkers = statuses.Count(s => s != TrackableBehaviour.Status.NO_POSE);

        foreach (MarkerAction marker in markerActionScripts)
        {
            marker.rotateCamera = rotateCamera;
            marker.mainCameraTransform = mainCameraTransform;
            marker.ARCameraHeight = ARCameraHeight;
        }

        UpdateUiText();
    }

    /// <summary>
    /// Updates the UI text with how many markers are currently being tracked
    /// </summary>
    private void UpdateUiText()
    {
        uiText.text = string.Format("{0} Marker{1} Tracking", numTrackedMarkers, numTrackedMarkers == 1 ? "" : "s");
        uiText.color = numTrackedMarkers == 0 ? Color.black : Color.green;
    }
}
