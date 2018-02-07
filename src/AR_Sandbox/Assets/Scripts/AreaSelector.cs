using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSelector : MonoBehaviour {
	public RectTransform lowerLeft, lowerRight, upperLeft, upperRight;

	// Use this for initialization
	void Start () {
		if (lowerLeft == null || lowerRight == null || upperLeft == null || upperRight == null) {
			Debug.LogError ("AreaSelector: a corner handle has not been assigned!");
		}
	}
	
	public void DragLowerLeft() {
		lowerLeft.position = Input.mousePosition;

		if (lowerLeft.position.y != lowerRight.position.y) {
			lowerRight.position = new Vector2(lowerRight.position.x, lowerLeft.position.y);
		}
	
		if (lowerLeft.position.x != upperLeft.position.x) {
			upperLeft.position = new Vector2(lowerLeft.position.x, upperLeft.position.y);
		}
	}

	public void DragUpperLeft() {
		upperLeft.position = Input.mousePosition;

		if (upperLeft.position.y != upperRight.position.y) {
			upperRight.position = new Vector2(upperRight.position.x, upperLeft.position.y);
		}

		if (upperLeft.position.x != lowerLeft.position.x) {
			lowerLeft.position = new Vector2(upperLeft.position.x, lowerLeft.position.y);
		}
	}

	public void DragLowerRight() {
		lowerRight.position = Input.mousePosition;

		if (lowerRight.position.y != lowerLeft.position.y) {
			lowerLeft.position = new Vector2(lowerLeft.position.x, lowerRight.position.y);
		}

		if (lowerRight.position.x != upperRight.position.x) {
			upperRight.position = new Vector2(lowerRight.position.x, upperRight.position.y);
		}
	}

	public void DragUpperRight() {
		upperRight.position = Input.mousePosition;

		if (upperRight.position.y != upperLeft.position.y) {
			upperLeft.position = new Vector2(upperLeft.position.x, upperRight.position.y);
		}

		if (upperRight.position.x != lowerRight.position.x) {
			lowerRight.position = new Vector2(upperRight.position.x, lowerRight.position.y);
		}
	}
}
