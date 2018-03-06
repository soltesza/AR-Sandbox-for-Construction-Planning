using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationManager : MonoBehaviour {
	private GameObject terrain;
	//private MaskTerrain MaskScript;

	[SerializeField]
	Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

	// Use this for initialization
	void Start () {
		terrain = GameObject.Find ("Terrain Mask");
		//MaskScript = (MaskTerrain)(terrain.GetComponent(typeof(MaskTerrain)));

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SubmitCalibration()
	{
		/*
		Vector3 UpperLeft, UpperRight, LowerLeft, LowerRight;
		float x1, y1, x2, y2, x3, y3, x4, y4;
		UpperRight = UIPanel.GetChild (0).GetChild (0).transform.position;
		LowerRight = UIPanel.GetChild (0).GetChild (1).transform.position;
		LowerLeft = UIPanel.GetChild (0).GetChild (2).transform.position;
		UpperLeft = UIPanel.GetChild (0).GetChild (3).transform.position;

		x1 = UpperLeft.x;
		y1 = UpperLeft.y;
		x2 = LowerLeft.x;
		y2 = LowerLeft.y;
		x3 = UpperRight.x;
		y3 = UpperRight.y;
		x4 = LowerRight.x;
		y4 = LowerRight.y;

		Debug.Log("("+ x1 + "," + y1 + ")" );
		Debug.Log("("+ x2 + "," + y2 + ")" );
		Debug.Log("("+ x3 + "," + y3 + ")" );
		Debug.Log("("+ x4 + "," + y4 + ")" );
		//Debug.Log(UIPanel.GetChild (0).GetChild(0).transform.position);
		//Debug.Log(UIPanel.GetChild (0).GetChild(0) );
		MaskScript.RePositionMesh (UpperLeft,LowerLeft,UpperRight,LowerRight);
		//MaskScript.ResizeMesh(10,10);
		Debug.Log("-------------------------------");
		*/
	}
}
