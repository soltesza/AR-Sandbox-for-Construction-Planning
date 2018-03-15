using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{

    public GameObject terrain;
    private TerrainGenerator terrainHeight;

    private float roadHeight;
    public static Mesh viewedModel;
    public GameObject Ground;
    public Material material;

    public GameObject road;
    private Road roadPoint;

    // Use this for initialization
    void Start()
    {
        // get terrain object mesh
        terrain = GameObject.Find("Terrain");
        road = GameObject.Find("Road");
        //MeshFilter viewedModelFilter = (MeshFilter)terrain.GetComponent("MeshFilter");
        //viewedModel = viewedModelFilter.mesh;

        // get terrain ground for raycasting
        Ground = terrain;

        // setup new material
        material = new Material(Shader.Find("Unlit/RoadShader"));
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public float getHeight()
    {
		// get altitude from ground
		Vector3 Direction = transform.position - Ground.transform.position;
		float altitude = Direction.y;
		//Debug.Log("Altitude: " + altitude);

		// get height from terrain
		roadHeight = Mathf.Infinity;
		RaycastHit hit;

		// when above terrain change to blue material
		// when below terrain change to red material
		if (Physics.Raycast(transform.position, Vector3.down, out hit))
			roadHeight = Vector3.Distance(hit.point, transform.position);
		//Debug.Log("Height: " + roadHeight);
		if (roadHeight == Mathf.Infinity)
		{
			material.color = Color.red;
			GetComponent<Renderer>().material = material;
		}
		else
		{
			material.color = Color.blue;
			GetComponent<Renderer>().material = material;
		}

        // testing get point from road
        roadPoint = road.GetComponent<Road>();
        Vector3[] positions = roadPoint.getRoadPoints();

		// testing get height function from terrain generator
		terrainHeight = terrain.GetComponent<TerrainGenerator>();
        int count = 0;
        foreach (Vector3 p in positions)
        {
            // Uncomment for cut/fill height data debug output 
            roadHeight = terrainHeight.GetHeightAtWorldPosition(p);
            Debug.Log("Position: " + count++ + " Height: " + roadHeight);
        }
        return roadHeight;
    }
}
