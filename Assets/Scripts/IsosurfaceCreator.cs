using UnityEngine;
using System.Collections;
using System.IO;

public class IsosurfaceCreator : MonoBehaviour
{
	
	FileBrowser mFileBrowser = null;
	Rect mFileBrowserRect = new Rect ();

	private GameObject mReferenceGameObject = null;
	public GameObject IsosurfaceObject {
		get {
			if (mReferenceGameObject == null) {
				mReferenceGameObject = new GameObject ("IsosurfaceObject");
			}
			return mReferenceGameObject;
		}
	}

	// Use this for initialization
	void Start ()
	{
		mFileBrowserRect.height = Screen.height * 0.5f;
		mFileBrowserRect.width = Screen.width * 05f;
		//mFileBrowserRect.center = new Vector2 (Screen.height * 0.5f, Screen.width * 0.5f);
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	private void OnSelectedDataFile (string datafile)
	{
		StreamReader sreader = new StreamReader (datafile);
		string datastring = sreader.ReadToEnd ();


		sreader.Close ();
	}
}
