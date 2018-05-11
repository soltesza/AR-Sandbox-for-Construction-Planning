using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; //Need this for calling UI scripts

public class CutAndFillManager : MonoBehaviour {

    // max points for the road
    const int MAXPOINTS = 20;

    // data table arrays
    int[] station       = new int[MAXPOINTS];
    int[] existGrade    = new int[MAXPOINTS];
    int[] propGrade     = new int[MAXPOINTS];
    int[] roadWidth     = new int[MAXPOINTS];
    int[] cutArea       = new int[MAXPOINTS];
    int[] fillArea      = new int[MAXPOINTS];
    int[] cutVolume     = new int[MAXPOINTS];
    int[] fillVolume    = new int[MAXPOINTS];
    int[] adjFillVolume = new int[MAXPOINTS];
    int[] algebraicSum  = new int[MAXPOINTS];
    int[] massOrdinate  = new int[MAXPOINTS];

    [SerializeField]
    Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

    [SerializeField]
    Text cutText0, //Will assign our cut Text to this variable so we can modify the text it displays.
         cutText1,
         cutText2,
         cutText3,
         cutText4;

    [SerializeField]
    Text fillText0, //Will assign our fill Text to this variable so we can modify the text it displays.
         fillText1,
         fillText2,
         fillText3,
         fillText4;

    public GameObject terrain;
    private TerrainGenerator terrainHeight;

    public GameObject road;
    private Road roadPoint;

    float timer = 0f;
    float waitingTime = 10f;

    void Start()
    {
        UIPanel.gameObject.SetActive(false); //make sure our pause menu is disabled when scene starts

        // get terrain object
        terrain = GameObject.Find("Terrain");
        // get height function from terrain generator
        terrainHeight = terrain.GetComponent<TerrainGenerator>();

        // get road object
        road = GameObject.Find("Road");
        // get point from road
        roadPoint = road.GetComponent<Road>();

        // TESTING
        updateStation();
        updateExistGrade();
        updatePropGrade();
        updateRoadWidth();
        updateCutArea();
        updateFillArea();
        updateCutVolume();
        updateFillVolume();
        updateAdjFillVolume();
        updateAlgebraicSum();
        updateMassOrdinate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {
        UIPanel.gameObject.SetActive(true); //turn on the pause menu
    }

    public void UnPause()
    {
        UIPanel.gameObject.SetActive(false); //turn off pause menu
    }

    void updateStation()
    {
        station[0] = 0;
        
        for (int i = 1; i < MAXPOINTS; i++)
        {
            station[i] = station[i - 1] + 100;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in station)
        {
            Debug.Log("STATION #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateExistGrade()
    {
        Vector3[] positions = roadPoint.GetRoadPoints();

        int i = 0;
        foreach (Vector3 p in positions)
        {
            existGrade[i] = (int) (10f * terrainHeight.GetHeightAtWorldPosition(p));
            i += 1;
            if (i >= MAXPOINTS)
                break;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in existGrade)
        {
            Debug.Log("EXISTING GRADE #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updatePropGrade()
    {
        for (int i = 0; i < MAXPOINTS; i++)
        {
            propGrade[i] = 0;
        }
        /*
        // DEBUG
        int count = 0;
        foreach (int s in propGrade)
        {
            Debug.Log("PROPOSED GRADE #" + count + " value " + s);
            count += 1;
        }
        */
    }

    void updateRoadWidth()
    {
        for (int i = 0; i < MAXPOINTS; i++)
        {
            roadWidth[i] = 120;
        }
        
        // DEBUG
        int count = 0;
        foreach (int s in roadWidth)
        {
            Debug.Log("ROAD WIDTH #" + count + " value " + s);
            count += 1;
        }
        
    }

    void updateCutArea()
    {

    }

    void updateFillArea()
    {

    }

    void updateCutVolume()
    {

    }

    void updateFillVolume()
    {

    }

    void updateAdjFillVolume()
    {

    }

    void updateAlgebraicSum()
    {

    }

    void updateMassOrdinate()
    {

    }
}
