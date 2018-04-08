using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationManager : MonoBehaviour {
	private Camera mainCamera;
	public float terrainSpeed = .01f;
	Vector3 move;
	[SerializeField]
	Transform UIPanel; //Will assign our panel to this variable so we can enable/disable it

	TerrainManager terrainManager;

	[SerializeField]
	GameObject terrain;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;

		terrainManager = GameObject.Find ("Terrain_Manager").GetComponent<TerrainManager> ();
	}
	// Update is called once per frame
	void Update () {
		SubmitCalibration ();
		if (ModeManager.dMode == DisplayMode.Calibrate) {
			//Have arrow keys adjust the terrain
			if (Input.GetKey (KeyCode.LeftArrow)) {
				moveTerrainLeft ();
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				moveTerrainRight ();
			
			}
			if (Input.GetKey (KeyCode.UpArrow)) {
				moveTerrainUp ();
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				moveTerrainDown ();
			}
		}
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log(terrain.transform.position);
		}
	}

	public void SubmitCalibration() {
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

		terrainManager.terrainMask.SetDimensions (panelLL, panelUR);
	}

	public void moveTerrainLeft()
	{
		float pos = terrain.transform.position.x;
		pos = terrain.transform.position.x - terrainSpeed;
		terrain.transform.position = new Vector3 (terrain.transform.position.x - terrainSpeed,terrain.transform.position.y,transform.position.z);
	}

	public void moveTerrainRight()
	{
		float pos = terrain.transform.position.x;
		pos = terrain.transform.position.x + terrainSpeed;
		terrain.transform.position = new Vector3 (terrain.transform.position.x + terrainSpeed,terrain.transform.position.y,transform.position.z);
	}

	public void moveTerrainUp()
	{
		float pos = terrain.transform.position.z;
		pos = terrain.transform.position.z + terrainSpeed;
		terrain.transform.position = new Vector3 (transform.position.x,terrain.transform.position.y,terrain.transform.position.z + terrainSpeed);
	}

	public void moveTerrainDown()
	{
		float pos = terrain.transform.position.z;
		pos = terrain.transform.position.z - terrainSpeed;
		terrain.transform.position = new Vector3 (transform.position.x,terrain.transform.position.y,terrain.transform.position.z - terrainSpeed);
	}
}
