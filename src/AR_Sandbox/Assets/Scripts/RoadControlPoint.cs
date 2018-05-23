using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadControlPoint : MonoBehaviour {
	public Road road;	// Reference to the road

	private TerrainManager terrainManager;


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

	// When user begins moving control point, push current control point positions to the undo stack so the action can be undone
	public void OnMouseDown() {
		road.PushStateToUndoStack ();
	}

	// Reposition the control point when the user starts dragging the point. Change y position if shift is held
	public void OnMouseDrag() {
		if (Input.GetKey (KeyCode.LeftShift)) { // Change height
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 pointPos = transform.position;
			float delta = mousePos.z - pointPos.z;
			transform.position = new Vector3 (pointPos.x, 
											  delta, 
											  pointPos.z);
			ConstrainHeight ();
		} else { // Change position
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			transform.position = new Vector3 (mousePos.x, transform.position.y, mousePos.z);
			ConstrainToTerrainMask ();
		}

		road.UpdateCurve ();
	}

	// Prevents the control point from moving outside the bounds of the terrain mask
	public void ConstrainToTerrainMask() {
		Vector3 position = transform.position;
		Vector4 bounds = terrainManager.GetMaskBounds ();

		position.x = position.x > bounds.w ? position.x : bounds.w;
		position.x = position.x < bounds.y ? position.x : bounds.y;
		position.z = position.z < bounds.x ? position.z : bounds.x;
		position.z = position.z > bounds.z ? position.z : bounds.z;

		transform.position = position;
	}

	// Prevents control point from moving above or below the bounds of the terrain
	private void ConstrainHeight() {
		Vector3 position = transform.position;
		float maxHeight = terrainManager.terrainGenerator.magnitude;

		position.y = position.y < maxHeight ? position.y : maxHeight;
		position.y = position.y > 0 ? position.y : 0;

		transform.position = position;
	}
}
