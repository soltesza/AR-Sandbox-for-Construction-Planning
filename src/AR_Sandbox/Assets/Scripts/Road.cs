using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Road : MonoBehaviour {
	public RoadControlPoint controlPointPrefab;			// Prefab that will be used to instantiate the control points
	public LineRenderer controlPointConnectorPrefab;	// Prefab that will be used to instantiate the line connecting the control points
	public TerrainGenerator terrain;					// Reference to the terrain

	private LineRenderer lineRenderer, controlPointConnector;
	private List<RoadControlPoint> controlPoints;		// List of control points that will affect the road
	private Stack<List<Vector3>> undoStack;				// Stack containing previous road states, used for undoing actions

	private const int SEGMENT_COUNT = 20;				// Number of line segments per curve, increase this number for a smoother line
    private const float THRESHOLD = 0.1f;               // Threshold where road considered level with terrain

	void Start () {
		if (!terrain) {
			Debug.Log ("Road.cs: terrain generator not specified");
		}

		lineRenderer = GetComponent<LineRenderer> ();
		controlPoints = new List<RoadControlPoint> ();

		CreateControlPoint (new Vector3(0f, 0f, 0f));
		CreateControlPoint (new Vector3 (1f, 4f, 6f));
		CreateControlPoint (new Vector3 (5f, -2f, 6f));
		CreateControlPoint (new Vector3(7f, 0f, 7f));


		undoStack = new Stack<List<Vector3>> ();

		lineRenderer.positionCount = SEGMENT_COUNT;

		controlPointConnector = CreateControlPointConnector ();

		UpdateCurve ();
	}

	// Instantiates the line connecting each control point
	LineRenderer CreateControlPointConnector() {
		LineRenderer connector = (LineRenderer)GameObject.Instantiate (controlPointConnectorPrefab);
		connector.transform.parent = this.transform;
		connector.positionCount = controlPoints.Count;

		return connector;
	}

	// Instantiates a control point at a given position
	void CreateControlPoint(Vector3 position) {
		RoadControlPoint newPoint = (RoadControlPoint)GameObject.Instantiate (controlPointPrefab);
		newPoint.transform.position = position;
		newPoint.road = this;
		newPoint.transform.parent = this.transform;
		controlPoints.Add(newPoint);
	}

	// Adds a new control point. Position is hardcoded to be one unit up and to the right of the current last point
	public void AddControlPoint(){
		Vector3 positionShift = new Vector3 (1f, 0f,1f);
		Vector3 position = controlPoints[controlPoints.Count - 1].transform.position + positionShift;

		CreateControlPoint (position);

		controlPointConnector.positionCount = controlPoints.Count;
		UpdateControlPointConnector();
	}

	// Remove the last control point
	public void RemoveControlPoint(){
		if (controlPoints.Count > 2) {
		
			Destroy (controlPoints [controlPoints.Count - 1].gameObject);
			controlPoints.RemoveAt (controlPoints.Count - 1);
			controlPointConnector.positionCount = controlPoints.Count;
			UpdateControlPointConnector();
		}
	}

	// Update control point connector to pass through new control point positions
	void UpdateControlPointConnector() {
		Vector3[] positions = new Vector3[controlPoints.Count]; 

		for (int i = 0; i < controlPoints.Count; i++) {
			positions [i] = controlPoints [i].transform.position;
		}

		controlPointConnector.SetPositions (positions);
	}

	//Update the coloration of the road to reflect whether it is above or below the terrain surface
	private void UpdateCurveMaterial() {
		int pixelCount = SEGMENT_COUNT * 4;

		Texture2D tex = new Texture2D (pixelCount, 1);

		for (int i = 0; i < pixelCount; i++) {
			float t = (1f / (float)pixelCount) * i;
			Vector3 pos = CalculateBezier (t, controlPoints);
			float terrainHeight = terrain.GetHeightAtWorldPosition(pos);
			Color color = terrainHeight > pos.y ? Color.red : Color.blue;

            // Road turns black if at or near level with terrain
            if (System.Math.Abs(terrainHeight - pos.y) <= THRESHOLD)
                color = Color.black;

			tex.SetPixel (i, 0, color);
		}

		tex.Apply ();
		GetComponent<Renderer>().material.mainTexture = tex;
	}

	// Recalculate the road given the current control point positions
	public void UpdateCurve() {
		Vector3[] positions = new Vector3[SEGMENT_COUNT]; 

		for (int i = 0; i < SEGMENT_COUNT; i++) {
			float t = i / (float)(SEGMENT_COUNT - 1);
			positions[i] = CalculateBezier (t, controlPoints);
		}

		lineRenderer.SetPositions (positions);
		UpdateControlPointConnector ();
		UpdateCurveMaterial ();
	}

	// Returns a single position along the curve
	// t is a float in [0, 1] representing the position along the curve to sample
	// points is the list of control points definignt he curve
	Vector3 CalculateBezier(float t, List<RoadControlPoint> points) {
		int count = points.Count;
		if (count > 2) {
			return (1 - t) * CalculateBezier(t, points.GetRange(0, count - 1)) + t * CalculateBezier(t, points.GetRange(1, count - 1));
		} else {
			return Vector3.Lerp (points [0].transform.position, points [1].transform.position, t);
		}
	}

	// Returns the list of vertices that define the line segemnt
    public Vector3[] GetRoadPoints() {
        Vector3[] positions = new Vector3[SEGMENT_COUNT];
        lineRenderer.GetPositions(positions);
        return positions;
    }

	// Returns the number of vertices making up the road
    public int GetNumRoadPoints() {
        return lineRenderer.positionCount;
    }

	// Disables the control points
    public void DisableControlPoints() {
        controlPointConnector.enabled = false;

        foreach (RoadControlPoint controlPoint in controlPoints) {
			controlPoint.gameObject.SetActive (false);
			controlPoint.ConstrainToTerrainMask();
		}
		UpdateCurve ();
	}

	// Enables the control points
	public void EnableControlPoints() {
        controlPointConnector.enabled = true;


		foreach (RoadControlPoint controlPoint in controlPoints) {
			controlPoint.gameObject.SetActive (true);
			controlPoint.ConstrainToTerrainMask();
		}
		UpdateCurve ();
	}

	// Undoes the last control point move
	public void Undo() {
		if (undoStack.Count > 0) {
			List<Vector3> positions = undoStack.Pop ();

			int count = positions.Count < controlPoints.Count ? positions.Count : controlPoints.Count;

			for (int i = 0; i < count; i++) {
				controlPoints [i].transform.position = positions [i];
			}
		}
	}

	// Adds the current position of the control points to the undo stack
	public void PushStateToUndoStack() {
		List<Vector3> positions = new List<Vector3>(controlPoints.Count);

		foreach(RoadControlPoint controlPoint in controlPoints) {
			positions.Add (controlPoint.transform.position);
		}

		undoStack.Push(positions);
	}
}
