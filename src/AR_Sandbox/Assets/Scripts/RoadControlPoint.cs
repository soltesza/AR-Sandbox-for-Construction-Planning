using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadControlPoint : MonoBehaviour {
	public Road road;

	// Use this for initialization
	void Start () {
		if (road == null) {
			road = GetComponentInParent<Road> ();
			if (road == null) {
				Debug.LogError ("RoadControlPoint: no road to control!");
			}
		}
	}

	public void OnMouseDrag() {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		transform.position = new Vector3 (mousePos.x, transform.position.y, mousePos.z);
		road.UpdateCurve ();
	}
}
