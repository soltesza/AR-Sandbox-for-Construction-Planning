using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use this to keep track of what state we are in
//Should other objects find the current state here?  Should this manager what is called for each state?

public enum DisplayMode : short {Depth, CutFill, Design, Calibrate};

public class ModeManager : MonoBehaviour {

    public static ModeManager instance = null;
    public static DisplayMode dMode = DisplayMode.Depth;
	private GameObject terrainGen, roadObj;
	private TerrainManager terrainManager;

	void Awake () {
        //Ensures that this object is a singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
	}

	void Start()
	{
		terrainManager = GameObject.Find ("Terrain_Manager").GetComponent<TerrainManager>();
		terrainGen = GameObject.Find ("Terrain");
		terrainGen.SetActive (false);
		roadObj = GameObject.Find ("Road");
		roadObj.SetActive (false);
	}

	
	void Update () {
		switch (dMode)
        {
            case DisplayMode.Depth:     //What should occur while in Depth Mode?
			    //Debug.Log("I am in Depth mode");
				terrainManager.SetTerrainTheme(TerrainManager.TerrainTheme.rainbow);
				enableTerrain();
				disableRoad();
                break;
            case DisplayMode.CutFill:   //What should occur while in CutFill Mode?
                 // Debug.Log("I am in CutFill mode");
				enableTerrain();
				terrainManager.SetTerrainTheme(TerrainManager.TerrainTheme.greyscale);
				//disableTerrain();
				enableRoad();
                break;
            case DisplayMode.Calibrate: //What should occur while in Calibrate Mode?
                // Debug.Log("I am in Calibrate mode");
				//disableTerrain();
				enableTerrain();
				disableRoad();
				break;
            case DisplayMode.Design:    //What should occur while in Design Mode?
                // Debug.Log("I am in Design mode");
				//disableTerrain();
				terrainManager.SetTerrainTheme(TerrainManager.TerrainTheme.greyscale);
				enableTerrain();
				enableRoad();
                break;

        }
	}

	//IF the terrain generator is disabled, enable it
	void enableTerrain()
	{
		if (!terrainGen.activeSelf)
			terrainGen.SetActive(true);
	}

	//IF the terrain generator is enabled, disable it
	void disableTerrain()
	{
		if (terrainGen.activeSelf)
			terrainGen.SetActive(false);
	}

	//IF the terrain generator is disabled, enable it
	void enableRoad()
	{
		if (!roadObj.activeSelf)
			roadObj.SetActive(true);
	}

	//IF the terrain generator is enabled, disable it
	void disableRoad()
	{
		if (roadObj.activeSelf)
			roadObj.SetActive(false);
	}

}
