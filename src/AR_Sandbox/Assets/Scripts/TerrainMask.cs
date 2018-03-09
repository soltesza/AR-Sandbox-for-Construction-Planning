using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
public class TerrainMask : MonoBehaviour {
	public float width;
	public float height;

	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;

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

	//set mesh length and width to a given value
	public void ResizeMesh(float width, float height) {
		vertices [0] = new Vector3 (0, 0, 0);
		vertices [1] = new Vector3 (0, 0, height);
		vertices [2] = new Vector3 (width, 0, height);
		vertices [3] = new Vector3 (width, 0, 0);

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	public void RePositionMesh(Vector3 LowerLeft, Vector3 UpperLeft, Vector3 UpperRight, Vector3 LowerRight) {
		vertices [0] = LowerLeft;
		vertices [1] = UpperLeft;
		vertices [2] = UpperRight;
		vertices [3] = LowerRight;

		mesh.vertices = vertices;
		mesh.RecalculateNormals ();
	}
		
	//set dimensions and position of mask by passing in world coordinates for lower left and upper right position. Y coordinates are ignored.
	public void SetDimensions(Vector3 lowerLeft, Vector3 upperRight) {
		transform.position = lowerLeft;
		Vector3 urOffset = upperRight - transform.position;

		ResizeMesh (urOffset.x, urOffset.z);
	}
}

