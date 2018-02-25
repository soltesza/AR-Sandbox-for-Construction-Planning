using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Road : MonoBehaviour {
	private LineRenderer lineRenderer;
	private List<Vector3> points;

	private const int SEGMENT_COUNT = 20; // Number of line segments per curve, increase this number for a smoother line

	// Use this for initialization
	void Start () {
		lineRenderer = GetComponent<LineRenderer> ();
		points = new List<Vector3> ();
		points.Add (new Vector3(0f, 0f, 0f));
		points.Add (new Vector3 (1f, 4f, 6f));
		points.Add (new Vector3 (5f, -2f, 6f));
		points.Add (new Vector3(7f, 0f, 7f));

		lineRenderer.positionCount = SEGMENT_COUNT;

		UpdateCurve ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void UpdateCurve() {
		for (int i = 0; i < SEGMENT_COUNT; i++) {
			float t = i / (float)SEGMENT_COUNT;

			Vector3 point = CalculateBezier (t, points);
			lineRenderer.SetPosition(i, point);
		}
	}

	Vector3 CalculateBezier(float t, List<Vector3> points) {
		int count = points.Count;
		if (count > 2) {
			return (1 - t) * CalculateBezier(t, points.GetRange(0, count - 1)) * t + t * CalculateBezier(t, points.GetRange(1, count - 1)) * t;
		} else {
			return Vector3.Lerp (points [0], points [1], t);
		}
	}
}
