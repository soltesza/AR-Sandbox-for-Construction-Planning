using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadControlPoint : MonoBehaviour {
	public Road road;

	private TerrainManager terrainManager;

	// Use this for initialization
	void Start () {
		if (road == null) {
			road = GetComponentInParent<Road> ();
			if (road == null) {
				Debug.LogError ("RoadControlPoint: no road to control!");
				this.enabled = false;
			}
		}

		terrainManager = GameObject.FindObjectOfType<TerrainManager> ();
		if (terrainManager == null) {
			Debug.LogError ("RoadControlPoint: cannot find terrain manager!");
			this.enabled = false;
		}
	}

	public void OnMouseDrag() {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		transform.position = new Vector3 (mousePos.x, transform.position.y, mousePos.z);
		ConstrainToTerrainMask ();
		road.UpdateCurve ();
	}

	// Prevents the control point from moving outside the bounds of the terrain mask
	private void ConstrainToTerrainMask() {
		Vector3 position = transform.position;
		Vector4 bounds = terrainManager.GetMaskBounds ();

		position.x = position.x > bounds.w ? position.x : bounds.w;
		position.x = position.x < bounds.y ? position.x : bounds.y;
		position.z = position.z < bounds.x ? position.z : bounds.x;
		position.z = position.z > bounds.z ? position.z : bounds.z;

		transform.position = position;
	}
}
