using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {
	public TerrainGenerator terrainGenerator; 	// Reference to the terrain generator
	public TerrainMask terrainMask;				// Reference to the terrain mask

	public enum TerrainTheme {rainbow, mountain, desert, greyscale};


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

	void Update () {
		SetTerrainTheme (TerrainTheme.desert);
	}

	// Set the height coloration profile of the terrain
	public void SetTerrainTheme(TerrainTheme theme) {
		Renderer renderer = terrainGenerator.transform.GetComponent<Renderer> ();

		if (theme == TerrainTheme.greyscale) {
			renderer.material.SetColor ("_Color0", new Color32(51, 51, 51, 255));
			renderer.material.SetColor ("_Color1", new Color32(102, 102, 102, 255));
			renderer.material.SetColor ("_Color2", new Color32(153, 153, 153, 255));
			renderer.material.SetColor ("_Color3", new Color32(204, 204, 204, 255));
			renderer.material.SetColor ("_Color4", new Color32(255, 255, 255, 255));
			renderer.material.SetColor ("_ColorB", Color.black);
			
		} else if (theme == TerrainTheme.mountain) {
			renderer.material.SetColor ("_Color0", new Color32(39, 101, 152, 255));
			renderer.material.SetColor ("_Color1", new Color32(100, 143, 52, 255));
			renderer.material.SetColor ("_Color2", new Color32(88, 87, 84, 255));
			renderer.material.SetColor ("_Color3", new Color32(150, 150, 150, 255));
			renderer.material.SetColor ("_Color4", new Color32(255, 255, 255, 255));
			renderer.material.SetColor ("_ColorB", Color.black);
			
		} else if (theme == TerrainTheme.desert) {
			renderer.material.SetColor ("_Color0", new Color32(229, 227, 180, 255));
			renderer.material.SetColor ("_Color1", new Color32(197, 189, 88, 255));
			renderer.material.SetColor ("_Color2", new Color32(143, 98, 69, 255));
			renderer.material.SetColor ("_Color3", new Color32(189, 135, 86, 255));
			renderer.material.SetColor ("_Color4", new Color32(203, 200, 171, 255));
			renderer.material.SetColor ("_ColorB", Color.black);

		} else {
			renderer.material.SetColor ("_Color0", Color.red);
			renderer.material.SetColor ("_Color1", Color.yellow);
			renderer.material.SetColor ("_Color2", Color.green);
			renderer.material.SetColor ("_Color3", Color.cyan);
			renderer.material.SetColor ("_Color4", Color.blue);
			renderer.material.SetColor ("_ColorB", Color.black);

		}
	}

	// Gets the bounds of the terrain mask, in world coordinates.
	// w = left bound, x = upper bound, y = right bound, z = lower bound
	public Vector4 GetMaskBounds() {
		Vector4 bounds = new Vector4 ();

		bounds.w = terrainMask.transform.position.x;
		bounds.x = terrainMask.transform.position.z + terrainMask.GetLength ();
		bounds.y = terrainMask.transform.position.x + terrainMask.GetWidth ();
		bounds.z = terrainMask.transform.position.z;

		return bounds;
	}
		
	public void SetMaxTerrainHeight(float maxHeight) {
        terrainGenerator.maxHeight = maxHeight;
	}

	public float GetMaxTerrainHeight() {
		return terrainGenerator.maxHeight;
	}

	public void SetMinTerrainHeight(float minHeight) {
		terrainGenerator.minHeight = minHeight;
	}

	public float GetMinTerrainHeight() {
		return terrainGenerator.minHeight;
	}
}
