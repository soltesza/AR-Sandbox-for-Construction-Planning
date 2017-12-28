using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTerrain : MonoBehaviour {

	public Texture2D heightmap;
	public float scale = 10;		// size of the resulting mesh
	public float magnitude = 1;		// maximum height of the resulting mesh

	// Use this for initialization

	void Start () {
		Mesh mesh = new Mesh ();
		Color[] pixels;

		float spacing = scale / heightmap.height;

		GetComponent<MeshFilter> ().mesh = mesh;
		pixels = heightmap.GetPixels ();			// Get array of pixels from heightmap

		// Initialize vertex and triangle arrays
		Vector3[] vertices = new Vector3[pixels.Length];
		int[] triangles = new int[(heightmap.height - 1) * (heightmap.width - 1) * 6];

		// Populate vertex array
		for (int i = 0; i < heightmap.height; i++) {
			for (int j = 0; j < heightmap.width; j++) {
				float y = pixels [j + heightmap.width * i].grayscale;		// Get Y value from greyscale value
				vertices [j + heightmap.width * i] = new Vector3 (j * spacing, y * magnitude, i * spacing);
			}
		}
			
		int idx = 0; // index for triangle array

		// Populate triangle array
		for (int i = 0; i < heightmap.height - 1; i++) {
			for (int j = 0; j < heightmap.width - 1; j++) {
				
				triangles [idx++] = j + heightmap.width * i; 			// '
				triangles [idx++] = j + heightmap.width * (1 + i);	 	// :
				triangles [idx++] = 1 + j + heightmap.width * i;	 	// :'

				triangles [idx++] = j + heightmap.width * (1 + i);		// .
				triangles [idx++] = 1 + j + heightmap.width * (1 + i); 	// ..
				triangles [idx++] = 1 + j + heightmap.width * i; 		// .:
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}


	void Update() {
		
	}
}
