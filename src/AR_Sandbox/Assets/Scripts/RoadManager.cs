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

    // Use this for initialization
    void Start()
    {
        // get terrain object mesh
        terrain = GameObject.Find("Terrain");
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

        // testing get height function from terrain generator
        terrainHeight = terrain.GetComponent<TerrainGenerator>();
        roadHeight = terrainHeight.GetHeightAtWorldPosition(transform.position);

        // Uncomment for cut/fill height data debug output 
        //Debug.Log("GetHeightAtWorldPosition: " + roadHeight);
    }

    public float getHeight()
    {
        return roadHeight;
    }
}
