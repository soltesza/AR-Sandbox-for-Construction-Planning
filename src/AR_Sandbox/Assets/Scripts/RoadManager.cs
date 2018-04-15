using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{

    public GameObject terrain;
    private TerrainGenerator terrainHeight;

    public GameObject road;
    private Road roadPoint;

    // Use this for initialization
    void Start()
    {
        // get terrain object
        terrain = GameObject.Find("Terrain");
        // get height function from terrain generator
        terrainHeight = terrain.GetComponent<TerrainGenerator>();

        // get road object
        road = GameObject.Find("Road");
        // get point from road
        roadPoint = road.GetComponent<Road>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public float getHeight()
    {
        Vector3[] positions = roadPoint.GetRoadPoints();

        float roadHeight = 0f;
        int count = 0;

        //Debug.Log("Height at World Position: " + terrainHeight.GetHeightAtWorldPosition(positions[0]));
        foreach (Vector3 p in positions)
        {
            // Uncomment for cut/fill height data debug output 
            roadHeight = 10f * terrainHeight.GetHeightAtWorldPosition(p);
            roadHeight = (float)(2f * (.5 * roadHeight * roadHeight) + 120f * roadHeight);
            //Debug.Log("Position: " + count++ + " Height: " + roadHeight);
        }
        return roadHeight;
    }
}
