using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationManager : MonoBehaviour {
	private Camera mainCamera;
	private GameObject terrain, terrainMask;
	private TerrainMask MaskScript;

	[SerializeField]
	Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

	// Use this for initialization
	void Start () {
		terrainMask = GameObject.Find ("Terrain Mask");
		terrain = GameObject.Find ("Terrain");
		mainCamera = Camera.main;
		MaskScript = (TerrainMask)(terrainMask.GetComponent(typeof(TerrainMask)));

	}
	
	// Update is called once per frame
	void Update () {
		SubmitCalibration ();
	}

	public void SubmitCalibration()
	{
		int panelWidth, panelHeight, terrainWidth, terrainHeight;
		Vector3 UpperRight, LowerLeft,UpperLeft,LowerRight;


		//Getting the positions of the corners of the UIPanel
		UpperRight = UIPanel.GetChild (0).GetChild (0).transform.position;
		LowerRight = UIPanel.GetChild (0).GetChild (1).transform.position;
		UpperLeft = UIPanel.GetChild (0).GetChild (3).transform.position;
		LowerLeft = UIPanel.GetChild (0).GetChild (2).transform.position;

		//Getting UI positions from camera positions
		Vector3 panelLL = mainCamera.ScreenToWorldPoint(LowerLeft);
		Vector3 panelUR = mainCamera.ScreenToWorldPoint(UpperRight);

		Vector3 panelUL = mainCamera.ScreenToWorldPoint(UpperLeft);
		Vector3 panelLR = mainCamera.ScreenToWorldPoint(LowerRight);


		MaskScript.RePositionMesh (new Vector3(panelLL.x,0,panelLL.z),
									new Vector3(panelUL.x,0,panelUL.z),
									new Vector3(panelUR.x,0,panelUR.z),
									new Vector3(panelLR.x,0,panelLR.z));


	}

}
