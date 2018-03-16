using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; //Need this for calling UI scripts

public class CutAndFillManager : MonoBehaviour {

    [SerializeField]
    Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

    [SerializeField]
    Text cutText; //Will assign our cut Text to this variable so we can modify the text it displays.

    [SerializeField]
    Text fillText; //Will assign our fill Text to this variable so we can modify the text it displays.

    public GameObject road;
    private RoadManager roadHeight;

    public GameObject terrain;
    private TerrainGenerator terrainHeight;

    float timer = 0f;
    float waitingTime = 5f;

    void Start()
    {
        UIPanel.gameObject.SetActive(false); //make sure our pause menu is disabled when scene starts
        road = GameObject.Find("Road");
        roadHeight = road.GetComponent<RoadManager>();

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
            float area = roadHeight.getHeight();
            if (area > 0)
            {
                cutText.text = "" + area; //Tells us the height of the road
                fillText.text = "" + 0; //Tells us the height of the road
            }
            else
            {
                cutText.text = "" + 0; //Tells us the height of the road
                fillText.text = "" + (-area); //Tells us the height of the road
            }
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
}
