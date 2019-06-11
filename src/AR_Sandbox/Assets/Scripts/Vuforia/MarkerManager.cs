using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class MarkerManager : MonoBehaviour
{
    public Text uiText;

    private List<ImageTargetBehaviour> markers;
    private List<TrackableBehaviour.Status> statuses;
    private int numTrackedMarkers;

    // Start is called before the first frame update
    void Start()
    {
        markers = FindObjectsOfType<ImageTargetBehaviour>().ToList();
        statuses = new List<TrackableBehaviour.Status>(markers.Count);
        numTrackedMarkers = 0;
    }

    // Update is called once per frame
    void Update()
    {
        statuses = markers.Select(m => m.CurrentStatus).ToList();
        numTrackedMarkers = statuses.Count(s => s != TrackableBehaviour.Status.NO_POSE);
        UpdateUiText();
    }

    private void UpdateUiText()
    {
        uiText.text = string.Format("{0} Marker{1} Tracking", numTrackedMarkers, numTrackedMarkers == 1 ? "" : "s");
        uiText.color = numTrackedMarkers == 0 ? Color.black : Color.green;
    }
}
