using UnityEngine;
using System.Collections;

public class IsosurfaceCreator : MonoBehaviour
{

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
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
