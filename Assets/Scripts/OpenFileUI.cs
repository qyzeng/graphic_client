using UnityEngine;
using System.Collections;

public class OpenFileUI : MonoBehaviour
{
	
	FileBrowser mFileBrowser = null;
	Rect mFileBrowserRect = new Rect ();
	Texture2D mRefTexture = null;
	Texture2D mRefHeightMap = null;
	bool mFileBrowserOn = false;
	public ModifiableTerrain mModifyTerrain;

	// Use this for initialization
	void Start ()
	{
		mFileBrowserRect.height = Screen.height * 0.5f;
		mFileBrowserRect.width = Screen.width * 05f;
		//mFileBrowserRect.center = new Vector2 (Screen.height * 0.5f, Screen.width * 0.5f);
		mFileBrowser = new FileBrowser (mFileBrowserRect, "Select image file", OnSelectedImageFile);

		mRefTexture = new Texture2D (1, 1);
		mRefHeightMap = new Texture2D (1, 1);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnGUI ()
	{
		if (GUILayout.Button ("Select Image")) {
			mFileBrowserOn = true;
		}
		if (mFileBrowserOn)
			mFileBrowser.OnGUI ();
	}

	private void OnSelectedImageFile (string path)
	{
		mFileBrowserOn = false;
		//Debug.Log (path);
		if (path.Length > 0) {
			System.Uri imageUri = new System.Uri (path);
			WWW imageResource = new WWW (imageUri.AbsoluteUri);
			while (!imageResource.isDone) {
			}
			imageResource.LoadImageIntoTexture (mRefTexture);
			imageResource.LoadImageIntoTexture (mRefHeightMap);
			int desiredWidth = mRefTexture.width.RoundUpToPowerTwo ();
			int desiredHeight = mRefTexture.height.RoundUpToPowerTwo ();
			Debug.Log ("Width: " + desiredWidth.ToString ());
			Debug.Log ("Height: " + desiredHeight.ToString ());
			TextureScale.Point (mRefTexture, desiredWidth, desiredHeight);
			TextureScale.Point (mRefHeightMap, desiredWidth, desiredHeight);
			if (mModifyTerrain != null) {
				mModifyTerrain.SetNewTerrainData (mRefHeightMap, mRefTexture);
			}
			/*
			Color[] splatTexColors = mRefTexture.GetPixels ();
			for (int i=0; i< splatTexColors.Length; ++i) {
				splatTexColors [i] = splatTexColors [i].grayscale * Color.green;
			}
			mRefTexture.SetPixels (splatTexColors);
			*/

		}
	}
}
