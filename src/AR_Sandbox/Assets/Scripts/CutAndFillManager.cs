using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; //Need this for calling UI scripts

public class CutAndFillManager : MonoBehaviour {

    [SerializeField]
    Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

    [SerializeField]
    Text heightText; //Will assign our height Text to this variable so we can modify the text it displays.

    public GameObject road;
    private RoadManager roadHeight;

    void Start()
    {
        UIPanel.gameObject.SetActive(false); //make sure our pause menu is disabled when scene starts
        road = GameObject.Find("Road");
    }

    void Update()
    {
        roadHeight = road.GetComponent<RoadManager>();
        heightText.text = "Height of Road: " + roadHeight.getHeight(); //Tells us the height of the road
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
