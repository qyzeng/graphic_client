using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Terrain))]
public class ModifiableTerrain : MonoBehaviour
{

	private TerrainData mTerrainData = null;
	private Texture2D mRefTexture = null;
	private Texture2D mRefHeightMap = null; 

	private float[,] mCurrentHeights = new float[1, 1];
	private float[,] mTargetHeights = new float[1, 1];

	public float InterpolationTime = 10f;

	public enum MODIFYSTATE
	{
		FREE = 0,
		BUSY = 1,
	}

	// Use this for initialization
	void Start ()
	{
		mTerrainData = this.GetComponent<Terrain> ().terrainData;
	}

	private void UpdateTerrain ()
	{
		int terrainWidth = mRefHeightMap.width;
		int terrainHeight = mRefHeightMap.height;
		terrainWidth = Mathf.ClosestPowerOfTwo (terrainWidth) < terrainWidth ? (Mathf.ClosestPowerOfTwo (terrainWidth) << 1) + 1 : Mathf.ClosestPowerOfTwo (terrainWidth) + 1;

		terrainHeight = Mathf.ClosestPowerOfTwo (terrainHeight) < terrainHeight ? (Mathf.ClosestPowerOfTwo (terrainHeight) << 1) + 1 : Mathf.ClosestPowerOfTwo (terrainHeight) + 1;
		mTargetHeights = new float[terrainWidth, terrainHeight];
		float[,] oldHeights = (float[,])mCurrentHeights.Clone ();
		mCurrentHeights = new float[terrainWidth, terrainHeight];
		int currWidth = Mathf.Min (mCurrentHeights.GetUpperBound (0), oldHeights.GetUpperBound (0)) + 1;
		int currHeight = Mathf.Min (mCurrentHeights.GetUpperBound (1), oldHeights.GetUpperBound (1)) + 1;
		for (int y=0; y < currHeight; ++y) {
			for (int x=0; x<currWidth; ++x) {
				mCurrentHeights [x, y] = oldHeights [x, y];
			}
		}

		for (int i=0; i<terrainHeight; ++i) {
			for (int j=0; j<terrainWidth; ++j) {
				mTargetHeights [j, i] = mRefHeightMap.GetPixel (i, j).grayscale;
			}
		}
		mTerrainData.heightmapResolution = Mathf.Max (terrainHeight, terrainWidth);
		mTerrainData.size = new Vector3 (terrainWidth, 100, terrainHeight);
		//mTerrainData.SetHeights (0, 0, mTargetHeights);
		this.transform.position = new Vector3 (-terrainWidth * 0.5f, 0, -terrainHeight * 0.5f);
		if (mTerrainData.splatPrototypes.Length == 0 || mTerrainData.splatPrototypes [0] == null || mTerrainData.splatPrototypes [0].texture == null) {
			SplatPrototype[] splatPrototypes = new SplatPrototype[1];
			SplatPrototype newSplat = new SplatPrototype ();
			newSplat.texture = new Texture2D (mRefTexture.width, mRefTexture.height);
			for (int x=0; x<mRefTexture.width; ++x) {
				for (int y=0; y<mRefTexture.height; ++y) {
					newSplat.texture.SetPixel (x, y, Color.grey);
				}
			}
			newSplat.tileSize = new Vector2 (mRefTexture.width, mRefTexture.height);
			splatPrototypes [0] = newSplat;
			mTerrainData.splatPrototypes = splatPrototypes;
		} else {
			mTerrainData.splatPrototypes [0].texture.Resize (mRefTexture.width, mRefTexture.height, TextureFormat.ARGB32, false);
			mTerrainData.splatPrototypes [0].texture.Apply ();
			mTerrainData.splatPrototypes [0].tileSize = new Vector2 (mRefTexture.width, mRefTexture.height);
		}
		
		//this.GetComponent<Terrain> ().materialTemplate.SetTexture ("Base", mTerrainData.splatPrototypes [0].texture);
		StartCoroutine (InterpolateTerrain ());
	}

	private void InterpolateTerrainValue (float interpolateValue)
	{
		for (int y=0; y<=mCurrentHeights.GetUpperBound(1); ++y) {
			for (int x=0; x<=mCurrentHeights.GetUpperBound(0); ++x) {
				mCurrentHeights [x, y] = Mathf.Lerp (mCurrentHeights [x, y], mTargetHeights [x, y], interpolateValue);
			}
		}
		mTerrainData.SetHeights (0, 0, mCurrentHeights);
		Texture2D splatTex = mTerrainData.splatPrototypes [0].texture;
		Color[] colors = splatTex.GetPixels ();
		Color[] targetColors = mRefTexture.GetPixels ();
		for (int i = 0; i < colors.Length; ++i) {
			colors [i] = Color.Lerp (colors [i], targetColors [i], interpolateValue);
		}
		splatTex.SetPixels (colors);
		splatTex.Apply ();
	}

	private IEnumerator InterpolateTerrain ()
	{
		float currentTime = 0f;
		while (currentTime <= InterpolationTime) {
			float interpolationValue = currentTime / InterpolationTime;
			InterpolateTerrainValue (interpolationValue);
			yield return new WaitForEndOfFrame ();
			currentTime += Time.deltaTime;
		}
		InterpolateTerrainValue (1f);
		yield return null;
	}

	public void SetNewTerrainData (Texture2D heightmap, Texture2D splatTexture)
	{
		StopCoroutine (InterpolateTerrain ());
		mRefHeightMap = heightmap;
		mRefTexture = splatTexture;
		UpdateTerrain ();
	}

}
