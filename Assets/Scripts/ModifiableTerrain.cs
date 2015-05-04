using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Terrain))]
public class ModifiableTerrain : MonoBehaviour
{

	private TerrainData mTerrainData = null;
	private Texture2D mRefTexture = null;
	private Texture2D mRefHeightMap = null; 

	// Use this for initialization
	void Start ()
	{
		mTerrainData = this.GetComponent<Terrain> ().terrainData;	
	}

	private void UpdateTerrain ()
	{
		int terrainWidth = mRefHeightMap.width;
		int terrainHeight = mRefHeightMap.height;
		float[,] heights = new float[terrainWidth, terrainHeight];
		for (int i=0; i<terrainHeight; ++i) {
			for (int j=0; j<terrainWidth; ++j) {
				heights [i, j] = mRefHeightMap.GetPixel (i, j).grayscale;
			}
		}
		mTerrainData.size = new Vector3 (terrainWidth, 1f, terrainHeight);
		mTerrainData.SetHeights (0, 0, heights);
		this.transform.position = new Vector3 (terrainWidth * 0.5f, 0, terrainHeight * 0.5f);
		SplatPrototype[] splatPrototypes = new SplatPrototype[1];
		splatPrototypes [0].texture = mRefTexture;
		mTerrainData.splatPrototypes = splatPrototypes;

	}

	public void SetNewTerrainData (Texture2D heightmap, Texture2D splatTexture)
	{
		mRefHeightMap = heightmap;
		mRefTexture = splatTexture;
		UpdateTerrain ();
	}

}
