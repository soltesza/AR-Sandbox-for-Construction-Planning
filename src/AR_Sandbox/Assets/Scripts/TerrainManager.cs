using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {
	public TerrainGenerator terrainGenerator;
	public TerrainMask terrainMask;


	// Use this for initialization
	void Start () {
		// Check to make sure a TerrainGenerator and TerrainMask are assigned, and if not try to assign one
		if (terrainGenerator == null) {
			terrainGenerator = GetComponentInChildren<TerrainGenerator> ();
			if (terrainGenerator == null) {
				Debug.LogError ("TerrainManager: No TerrainGenerator found!");
				this.enabled = false;
			}
		}

		if (terrainMask == null) {
			terrainMask = GetComponentInChildren<TerrainMask> ();
			if (terrainMask == null) {
				Debug.LogError ("TerrainManager: No TerrainMask found!");
				this.enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		terrainGenerator.UpdateMesh ();
	}
		
	void SetMaxTerrainHeight(float maxHeight) {
		terrainGenerator.maxHeight = maxHeight;
	}

	float GetMaxTerrainHeight() {
		return terrainGenerator.maxHeight;
	}

	void SetMinTerrainHeight(float minHeight) {
		terrainGenerator.minHeight = minHeight;
	}

	float getMinTerrainHeight() {
		return terrainGenerator.minHeight;
	}
}
