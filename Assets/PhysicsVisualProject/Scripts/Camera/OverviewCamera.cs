using UnityEngine;
using System.Collections;

public class OverviewCamera : BaseCamera
{
	private float _orthoSize = 64f;

	public float OrthoSize {
		get {
			return _orthoSize;
		}
		set {
			if (_orthoSize != value) {
				_orthoSize = value;

			}
		}
	}

	private void RefreshCameras ()
	{
		Camera[] allCameras = GetComponentsInChildren<Camera> ();
		foreach (Camera cam in allCameras) {
			cam.orthographic = true;
			cam.orthographicSize = _orthoSize;
		}
	}

	private void OnDisable ()
	{
		Camera[] allCameras = GetComponentsInChildren<Camera> ();
		foreach (Camera cam in allCameras) {
			cam.orthographic = false;
			cam.fieldOfView = FOV;
		}
	}

	protected override void OnEnable ()
	{
		RefreshCameras ();
		//base.OnEnable ();
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		this.transform.LookAt (TargetNode.position);
	}

	public override void SetOverrideRotation (Quaternion rotation)
	{
		transform.rotation = rotation;
		//base.SetOverrideRotation (rotation);
	}
}
