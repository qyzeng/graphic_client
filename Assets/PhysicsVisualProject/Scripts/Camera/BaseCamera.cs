using UnityEngine;
using System.Collections;

public class BaseCamera : MonoBehaviour
{
	public Transform TargetNode;
	public float FOV = 60f;
    
	protected virtual void OnEnable ()
	{
		Camera[] thisCameras = GetComponentsInChildren<Camera> ();
		foreach (Camera cam in thisCameras) {
			cam.orthographic = false;
			cam.fieldOfView = FOV;
		}
	}

	// Use this for initialization
	protected virtual void Start ()
	{
	
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
	
	}
    
	public virtual void RotateCamera (Vector2 rotateVector)
	{
    
	}
    
	public virtual void ResetCamera ()
	{
	}
    
	public virtual void Zoom (float delta)
	{
	}

	public virtual void SetOverrideRotation (Quaternion rotation)
	{

	}
}
