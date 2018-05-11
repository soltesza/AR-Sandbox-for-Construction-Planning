using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; //Need this for calling UI scripts

public class CutAndFillManager : MonoBehaviour {

    // data table arrays
    int[] station       = new int[20];
    int[] existGrade    = new int[20];
    int[] propGrade     = new int[20];
    int[] roadWidth     = new int[20];
    int[] cutArea       = new int[20];
    int[] fillArea      = new int[20];
    int[] cutVolume     = new int[20];
    int[] fillVolume    = new int[20];
    int[] adjFillVolume = new int[20];
    int[] algebraicSum  = new int[20];
    int[] massOrdinate  = new int[20];

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

    public GameObject road;
    private RoadManager roadArea;

    public GameObject terrain;
    private TerrainGenerator terrainHeight;

    float timer = 0f;
    float waitingTime = 5f;

    void Start()
    {
        UIPanel.gameObject.SetActive(false); //make sure our pause menu is disabled when scene starts
        road = GameObject.Find("Road");
        roadArea = road.GetComponent<RoadManager>();

        // get terrain object
        terrain = GameObject.Find("Terrain");
        // testing get height function from terrain generator
        terrainHeight = terrain.GetComponent<TerrainGenerator>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > waitingTime)
        {
            timer = 0f;

            float[] area = roadArea.getRoadAreas();
            int count = 0;
            int countBy = area.Length / 4;

            // station 0
            if (area[count] < 1000)
            {
                cutText0.text = "" + area[count]; 
                fillText0.text = "" + 0; 
            }
            else
            {
                cutText0.text = "" + 0; 
                fillText0.text = "" + (-area[count]); 
            }
            count += countBy - 1;

            // station 1
            if (area[count] < 1000)
            {
                cutText1.text = "" + area[count]; 
                fillText1.text = "" + 0; 
            }
            else
            {
                cutText1.text = "" + 0; 
                fillText1.text = "" + (-area[count]); 
            }
            count += countBy;

            // station 2
            if (area[count] < 1000)
            {
                cutText2.text = "" + area[count]; 
                fillText2.text = "" + 0; 
            }
            else
            {
                cutText2.text = "" + 0; 
                fillText2.text = "" + (-area[count]); 
            }
            count += countBy;

            // station 3
            if (area[count] < 1000)
            {
                cutText3.text = "" + area[count]; 
                fillText3.text = "" + 0; 
            }
            else
            {
                cutText3.text = "" + 0; 
                fillText3.text = "" + (-area[count]); 
            }
            count += countBy;

            // station 4
            if (area[count] < 1000)
            {
                cutText4.text = "" + area[count]; 
                fillText4.text = "" + 0; 
            }
            else
            {
                cutText4.text = "" + 0; 
                fillText4.text = "" + (-area[count]); 
            }
            count += countBy;
        }
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

    }

    void updateExistGrade()
    {

    }

    void updatePropGrade()
    {

    }

    void updateRoadWidth()
    {

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
