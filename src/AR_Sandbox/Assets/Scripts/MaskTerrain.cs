using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskTerrain : MonoBehaviour {
	public float width;
	public float height;

	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;

	// Use this for initialization
	void Start () {
		mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		vertices = new Vector3[4];
		triangles = new int[6];

		vertices [0] = new Vector3 (0, 0, 0);
		vertices [1] = new Vector3 (0, 0, 1);
		vertices [2] = new Vector3 (1, 0, 1);
		vertices [3] = new Vector3 (1, 0, 0);

		triangles [0] = 0;
		triangles [1] = 1;
		triangles [2] = 2;
		triangles [3] = 0;
		triangles [4] = 2;
		triangles [5] = 3;

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	void Update() {
		ResizeMesh (width, height);
	}

	public void ResizeMesh(float width, float height) {
		vertices [0] = new Vector3 (0, 0, 0);
		vertices [1] = new Vector3 (0, 0, height);
		vertices [2] = new Vector3 (width, 0, height);
		vertices [3] = new Vector3 (width, 0, 0);

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}
}
