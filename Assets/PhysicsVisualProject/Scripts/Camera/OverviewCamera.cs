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

	protected override void OnEnable ()
	{
		RefreshCameras ();
		//base.OnEnable ();
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.transform.LookAt (TargetNode.position);
	}
}
