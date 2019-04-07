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
    public Text uiText;

    private List<ImageTargetBehaviour> markers;
    private List<TrackableBehaviour.Status> statuses;
    private int numTrackedMarkers;
    
    void Start()
    {
        markers = FindObjectsOfType<ImageTargetBehaviour>().ToList();
        statuses = new List<TrackableBehaviour.Status>(markers.Count);
        numTrackedMarkers = 0;
    }
    
    void Update()
    {
        statuses = markers.Select(m => m.CurrentStatus).ToList();
        numTrackedMarkers = statuses.Count(s => s != TrackableBehaviour.Status.NO_POSE);
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
