using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Road : MonoBehaviour {
	public RoadControlPoint controlPointPrefab;
	public LineRenderer controlPointConnectorPrefab;
	public TerrainGenerator terrain;

	private LineRenderer lineRenderer, controlPointConnector;
	private List<RoadControlPoint> controlPoints;
	private Stack<List<Vector3>> undoStack;

	private const int SEGMENT_COUNT = 20; // Number of line segments per curve, increase this number for a smoother line

	// Use this for initialization
	void Start () {
		if (!terrain) {
			Debug.Log ("Road.cs: terrain genrator not specified");
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

	LineRenderer CreateControlPointConnector() {
		LineRenderer connector = (LineRenderer)GameObject.Instantiate (controlPointConnectorPrefab);
		connector.transform.parent = this.transform;
		connector.positionCount = controlPoints.Count;

		return connector;
	}

	void CreateControlPoint(Vector3 position) {
		RoadControlPoint newPoint = (RoadControlPoint)GameObject.Instantiate (controlPointPrefab);
		newPoint.transform.position = position;
		newPoint.road = this;
		newPoint.transform.parent = this.transform;
		controlPoints.Add(newPoint);
	}

	public void AddControlPoint(){
		RoadControlPoint newPoint = (RoadControlPoint)GameObject.Instantiate (controlPointPrefab);
		Vector3 positionShift = new Vector3 (1f, 0f,1f);
		newPoint.transform.position = controlPoints[controlPoints.Count - 1].transform.position + positionShift;
		newPoint.road = this;
		newPoint.transform.parent = this.transform;
		controlPoints.Add(newPoint);

		controlPointConnector.positionCount = controlPoints.Count;
		UpdateControlPointConnector();
	}

	public void RemoveControlPoint(){
		if (controlPoints.Count > 2) {
		
			Destroy (controlPoints [controlPoints.Count - 1].gameObject);
			controlPoints.RemoveAt (controlPoints.Count - 1);
			controlPointConnector.positionCount = controlPoints.Count;
			UpdateControlPointConnector();
		}
	}

	void UpdateControlPointConnector() {
		Vector3[] positions = new Vector3[controlPoints.Count]; 

		for (int i = 0; i < controlPoints.Count; i++) {
			positions [i] = controlPoints [i].transform.position;
		}

		controlPointConnector.SetPositions (positions);
	}

	public void UpdateCurve() {
		Vector3[] positions = new Vector3[SEGMENT_COUNT]; 
		Texture2D tex = new Texture2D (SEGMENT_COUNT + 1, 1);

		for (int i = 0; i < SEGMENT_COUNT; i++) {
			float t = i / (float)(SEGMENT_COUNT - 1);
			positions[i] = CalculateBezier (t, controlPoints);

			float terrainHeight = terrain.GetHeightAtWorldPosition(positions[i]);
			Color color = terrainHeight > positions[i].y ? Color.red : Color.blue;

			tex.SetPixel (i, 0, color);
		}

		lineRenderer.SetPositions (positions);
		UpdateControlPointConnector ();

		tex.Apply ();
		GetComponent<Renderer>().material.mainTexture = tex;
	}

	Vector3 CalculateBezier(float t, List<RoadControlPoint> points) {
		int count = points.Count;
		if (count > 2) {
			return (1 - t) * CalculateBezier(t, points.GetRange(0, count - 1)) + t * CalculateBezier(t, points.GetRange(1, count - 1));
		} else {
			return Vector3.Lerp (points [0].transform.position, points [1].transform.position, t);
		}
	}

    public Vector3[] GetRoadPoints() {
        Vector3[] positions = new Vector3[SEGMENT_COUNT];
        lineRenderer.GetPositions(positions);
        return positions;
    }

    public int GetNumRoadPoints() {
        return lineRenderer.positionCount;
    }

    public void DisableControlPoints() {
        controlPointConnector.enabled = false;

        foreach (RoadControlPoint controlPoint in controlPoints) {
			controlPoint.gameObject.SetActive (false);
			controlPoint.ConstrainToTerrainMask();
		}
		UpdateCurve ();
	}

	public void EnableControlPoints() {
        controlPointConnector.enabled = true;

		foreach (RoadControlPoint controlPoint in controlPoints) {
			controlPoint.gameObject.SetActive (true);
			controlPoint.ConstrainToTerrainMask();
		}
		UpdateCurve ();
	}

	public void Undo() {
		if (undoStack.Count > 0) {
			List<Vector3> positions = undoStack.Pop ();

			int count = positions.Count < controlPoints.Count ? positions.Count : controlPoints.Count;

			for (int i = 0; i < count; i++) {
				controlPoints [i].transform.position = positions [i];
			}
		}
	}

	public void PushStateToUndoStack() {
		List<Vector3> positions = new List<Vector3>(controlPoints.Count);

		foreach(RoadControlPoint controlPoint in controlPoints) {
			positions.Add (controlPoint.transform.position);
		}

		undoStack.Push(positions);
	}
}
