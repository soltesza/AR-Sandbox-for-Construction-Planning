using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class UpdateTerrain : MonoBehaviour {
	public Texture2D heightmap;
	public GameObject depthSourceManager;
	public float scale = 10;		// Size of the resulting mesh
	public float magnitude = 1;		// Maximum height of the resulting mesh

	private Mesh mesh;
	private float spacing;			// The distance between vertices in the mesh
	private Vector3[] vertices;
	private int[] triangles;
	private KinectSensor sensor;
	private CoordinateMapper mapper;
	private DepthSourceManager manager;

	private const int downsampleSize = 4;
	private const float maxDist = 3000;


	void Start () {
		sensor = KinectSensor.GetDefault ();

		if (sensor != null) {
			mapper = sensor.CoordinateMapper;
			manager = depthSourceManager.GetComponent<DepthSourceManager> ();
			if (manager == null) {
				return;
			}

			mesh = new Mesh ();		// Initialize mesh
			GetComponent<MeshFilter> ().mesh = mesh;

			FrameDescription frameDesc = sensor.DepthFrameSource.FrameDescription;

			spacing = scale / heightmap.height;

			CreateMesh (frameDesc.Width / downsampleSize, frameDesc.Height / downsampleSize);

			if (!sensor.IsOpen) {
				sensor.Open ();
			}
		}
	}

    // Create a new mesh by generating a set of vertices and triangles
	void CreateMesh(int x, int y) {
        // Initialize vertex and triangle arrays
        vertices = new Vector3[x * y];
		triangles = new int[(y - 1) * (x - 1) * 6];

		// Populate vertex array
		for (int i = 0; i < y; i++) {
			for (int j = 0; j < x; j++) {
				vertices [j + x * i] = new Vector3 (j * spacing, 0, i * spacing);
			}
		}

		int idx = 0; // index for triangle array

		// Populate triangle array
		for (int i = 0; i < y - 1; i++) {
			for (int j = 0; j < x - 1; j++) {

				triangles [idx++] = j + x * i; 				// '
				triangles [idx++] = j + x * (1 + i);		// :
				triangles [idx++] = 1 + j + x * i;	 		// :'

				triangles [idx++] = j + x * (1 + i);		// .
				triangles [idx++] = 1 + j + x * (1 + i); 	// ..
				triangles [idx++] = 1 + j + x * i; 			// .:
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	//update the terrain mesh with height data from Kinect sensor
	void UpdateMesh(ushort[] heightData) {
		var frameDesc = sensor.DepthFrameSource.FrameDescription;

		// Populate vertex array
		for (int i = 0; i < frameDesc.Height/downsampleSize; i++) {
			for (int j = 0; j < frameDesc.Width/downsampleSize; j++) {
				ushort y = heightData [j * downsampleSize + frameDesc.Width * i * downsampleSize];		// Get Y value from Kinect height data
				float yNorm = (float)y / maxDist; 														// Normalize height	
				vertices [j + frameDesc.Width/downsampleSize * i] = new Vector3 (j * spacing, yNorm * magnitude, i * spacing);
			}
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	void Update() {
		if (sensor != null) {
			UpdateMesh (manager.GetData ());
		}
	}

	void OnApplicationQuit() {
		if (mapper != null) {
			mapper = null;
		}

		if (sensor != null) {
			if (sensor.IsOpen) {
				sensor.Close();
			}

			sensor = null;
		}
	}
}
