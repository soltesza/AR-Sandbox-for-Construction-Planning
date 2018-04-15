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

    public float[] getRoadAreas()
    {
        Vector3[] positions = roadPoint.GetRoadPoints();

        float[] roadHeight = new float[roadPoint.GetNumRoadPoints()];
        float[] roadAreas = new float[roadPoint.GetNumRoadPoints()];
        int count = 0;

        foreach (Vector3 p in positions)
        {
            roadHeight[count] = 10f * terrainHeight.GetHeightAtWorldPosition(p);
            roadAreas[count] = (float)(2f * (.5 * roadHeight[count] * roadHeight[count]) + 120f * roadHeight[count]);

            //Debug.Log("Height at World Position " + count + ": " + terrainHeight.GetHeightAtWorldPosition(p));
            //Debug.Log("Position: " + count + " Height: " + roadHeight[count]);

            count++;
        }

        return roadAreas;
    }
}
