using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightView : MonoBehaviour {
	public LineRenderer roadLine, terrainLine;
	public float scale; // Controls the length of the line
	public float magnitude; // Controls the height of the line

	private TerrainManager terrainManager; // Reference to the terrain manager in the scene
	private Road road; // Reference tot he road in the scene
	private float spacing; // The actual distance between control points, determined by scale and number of road segments


	void Start () {
		road = GameObject.FindObjectOfType<Road> ();
		if (road == null) {
			road = GetComponentInParent<Road> ();
			if (road == null) {
				Debug.LogError ("HeightView: cannot find road!");
				this.enabled = false;
			}
		}

		terrainManager = GameObject.FindObjectOfType<TerrainManager> ();
		if (terrainManager == null) {
			Debug.LogError ("HeightView: cannot find terrain manager!");
			this.enabled = false;
		}

		if (roadLine == null || terrainLine == null) {
			Debug.LogError ("HeightView: one or more line renderers not assigned!");
			this.enabled = false;
		}

		spacing = scale / road.GetRoadPoints ().Length;
	}

	// Update the terrain and road lines
	void UpdateLines() {
		Vector3[] roadPoints = road.GetRoadPoints ();
		Vector3[] roadLinePoints = new Vector3[roadPoints.Length];
		Vector3[] terrainLinePoints = new Vector3[roadPoints.Length];
		roadLine.positionCount = roadPoints.Length;
		terrainLine.positionCount = roadPoints.Length;

		for(int i = 0; i < roadPoints.Length; i++) { // Update terrain line by querying terrain height
			roadLinePoints [i] = new Vector3 (i * spacing, 0f, magnitude * roadPoints[i].y) + transform.position;
			terrainLinePoints [i] = new Vector3 (i * spacing, 0f, magnitude * terrainManager.terrainGenerator.GetHeightAtWorldPosition(roadPoints[i])) + transform.position;
		}

		roadLine.SetPositions (roadLinePoints);
		terrainLine.SetPositions (terrainLinePoints);
	}
		
	void Update () {
		UpdateLines ();
	}
}
