using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use this to keep track of what state we are in
//Should other objects find the current state here?  Should this manager what is called for each state?

public enum DisplayMode : short {Depth, CutFill, Design, Calibrate};

public class ModeManager : MonoBehaviour {

    public static ModeManager instance = null;
    public static DisplayMode dMode = DisplayMode.Depth;

	void Awake () {
        //Ensures that this object is a singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
	}
	
	
	void Update () {
		switch (dMode)
        {
            case DisplayMode.Depth:     //What should occur while in Depth Mode?
               // Debug.Log("I am in Depth mode");
                break;
            case DisplayMode.CutFill:   //What should occur while in CutFill Mode?
                 // Debug.Log("I am in CutFill mode");
                break;
            case DisplayMode.Calibrate: //What should occur while in Calibrate Mode?
                 // Debug.Log("I am in Calibrate mode");
                break;
            case DisplayMode.Design:    //What should occur while in Design Mode?
                 // Debug.Log("I am in Design mode");
                break;

        }
	}
}
