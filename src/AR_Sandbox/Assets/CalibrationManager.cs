using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationManager : MonoBehaviour {

	[SerializeField]
	Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SubmitCalibration()
	{
		Debug.Log("Submit Button Clicked");
	}
}
