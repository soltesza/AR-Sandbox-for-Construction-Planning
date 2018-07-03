using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use this to keep track of what state we are in
public enum DisplayMode : short {Depth, CutFill, Design, Calibrate};

public class ModeManager : MonoBehaviour {

    public static ModeManager instance = null;		      //Used to ensure object is Singleton
    public static DisplayMode dMode = DisplayMode.Depth;  //Current Display Mode

	[SerializeField]
	GameObject HeightView; 
	[SerializeField]
	Road road; 
	[SerializeField]
	TerrainManager terrainManager; 

	void Awake () {
        //Ensures that this object is a singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
	}

	void Start()
	{

	}

	
	void Update () {
		switch (dMode) {
            case DisplayMode.Depth:     //What should occur while in Depth Mode
				terrainManager.SetTerrainTheme(TerrainManager.TerrainTheme.rainbow);
				road.gameObject.SetActive (false);
                break;
			case DisplayMode.CutFill:   //What should occur while in CutFill Mode
				terrainManager.SetTerrainTheme (TerrainManager.TerrainTheme.greyscale);
				road.gameObject.SetActive (true);
				road.DisableControlPoints ();
				break;
            case DisplayMode.Calibrate: //What should occur while in Calibrate Mode
				terrainManager.SetTerrainTheme(TerrainManager.TerrainTheme.rainbow);
				road.gameObject.SetActive (false);
				break;
			case DisplayMode.Design:    //What should occur while in Design Mode
				terrainManager.SetTerrainTheme (TerrainManager.TerrainTheme.greyscale);
				road.gameObject.SetActive (true);
				road.EnableControlPoints ();
				if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKeyDown (KeyCode.Z)) {
					road.Undo ();		
				}

				if (Input.GetKeyDown (KeyCode.LeftShift)) {
					HeightView.SetActive (true);
				}
				if (Input.GetKeyUp (KeyCode.LeftShift)) {
					HeightView.SetActive (false);
				}
                if (Input.GetKey(KeyCode.H)) {
                    HeightView.SetActive(!HeightView.activeSelf);
                }

                break;
        }
	}
		
}
