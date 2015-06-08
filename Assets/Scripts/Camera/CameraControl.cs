using UnityEngine;
using System.Collections;
using WP.Controller;

public class CameraControl : MonoBehaviour, IControlListener
{
	public enum CameraType
	{
		FREE_CAM,
		CHASE_CAM,
		ORBITAL_CAM,
		OVERHEAD_CAM,
		FPS_CAM,
	}
    
	public delegate void CameraTypeChangedHandler (CameraType newCamType);
	public event CameraTypeChangedHandler OnCameraTypeChanged;
	public delegate void CameraTargetChangedHandler (GameObject target);
	public event CameraTargetChangedHandler OnCameraTargetChanged;
    
	private ChaseCamera _chaseCamera = null;
	private OrbitalCamera _orbitalCamera = null;
	private FirstPersonCamera _FPSCamera = null;
	[SerializeField]
	private CameraType
		_camType = CameraType.CHASE_CAM;
	private BaseCamera _currentCamera = null;
	[SerializeField]
	private GameObject
		_lookAtTarget = null;

	public float CameraRotationSpeed = 30f;
	private float _horizontalInput = 0f;
	private float _verticalInput = 0f;
    
	private System.Collections.Generic.List<IController> _controllerList = new System.Collections.Generic.List<IController> ();
    
	public GameObject LookAtTarget {
		get {
			return _lookAtTarget;
		}
		set {
			if (_lookAtTarget != value) {
				_lookAtTarget = value;
				if (OnCameraTargetChanged != null) {
					OnCameraTargetChanged (_lookAtTarget);
				}
			}
		}
	}
    
	public CameraType CamType {
		get {
			return _camType;
		}
		set {
			if (_camType != value) {
				_camType = value;
				if (OnCameraTypeChanged != null) {
					OnCameraTypeChanged (_camType);
				}
			}
		}
	}
    
	public ChaseCamera ChaseCam {
		get {
			if (_chaseCamera == null) {
				_chaseCamera = gameObject.GetComponent<ChaseCamera> () == null ? gameObject.AddComponent<ChaseCamera> () : gameObject.GetComponent<ChaseCamera> ();
			}
			return _chaseCamera;
		}
	}
    
	public OrbitalCamera OrbitalCam {
		get {
			if (_orbitalCamera == null) {
				_orbitalCamera = gameObject.GetComponent<OrbitalCamera> () == null ? gameObject.AddComponent<OrbitalCamera> () : gameObject.GetComponent<OrbitalCamera> ();
			}
			return _orbitalCamera;
		}
	}

	public FirstPersonCamera FPSCam {
		get {
			if (_FPSCamera == null) {
				_FPSCamera = gameObject.GetComponent<FirstPersonCamera> () == null ? gameObject.AddComponent<FirstPersonCamera> () : gameObject.GetComponent<FirstPersonCamera> ();
			}
			return _FPSCamera;
		}
	}
    
    #region Public Methods
	public void RotateCameraHorizontal (float val)
	{
		_horizontalInput = val;
	}
    
	public void RotateCameraVertical (float val)
	{
		_verticalInput = val;
	}
    
	public void ResetCamera ()
	{
		if (_currentCamera != null) {
			_currentCamera.ResetCamera ();
		}
	}
    
	public void Zoom (float delta)
	{
		if (_currentCamera != null) {
			_currentCamera.Zoom (delta);
		}
	}
    
	public void ZoomIn (float delta)
	{
		if (_currentCamera != null) {
			_currentCamera.Zoom (delta);
		}
	}
    
	public void ZoomOut (float delta)
	{
		if (_currentCamera != null) {
			_currentCamera.Zoom (-delta);
		}
	}
    
	public void AddController (IController controller)
	{
		controller.OnControllerCommandsFired += OnControlCommand;
		_controllerList.Add (controller);
	}
    
	public void RemoveController (IController controller)
	{
		controller.OnControllerCommandsFired -= OnControlCommand;
		if (_controllerList.Contains (controller)) {
			_controllerList.Remove (controller);
		}
	}

	public void OverrideRotation (Quaternion rotation)
	{
		if (_currentCamera != null) {
			_currentCamera.SetOverrideRotation (rotation);
		} else {
			StartCoroutine (SetOverrideRotationAfterFrames (10, rotation));
		}
	}
    #endregion
    
    #region Private Methods
	private IEnumerator SetOverrideRotationAfterFrames (int framecount, Quaternion rotation)
	{
		for (int i=0; i<framecount; ++i) {
			yield return new WaitForEndOfFrame ();
		}
		if (_currentCamera != null) {
			_currentCamera.SetOverrideRotation (rotation);
		}
	}

	private void OnControlCommand (System.Collections.Generic.List<CommandFiredEventArgs> args)
	{
		foreach (CommandFiredEventArgs commandArg in args) {
			switch ((COMMAND_TYPE)commandArg.Command) {
			case COMMAND_TYPE.CAMERA_HORIZONTAL:
				RotateCameraHorizontal (commandArg.Arguments [0] != null ? (float)(commandArg.Arguments [0]) : 0);
				break;
			case COMMAND_TYPE.CAMERA_VERTICAL:
				RotateCameraVertical (commandArg.Arguments [0] != null ? (float)(commandArg.Arguments [0]) : 0);
				break;
			case COMMAND_TYPE.CAMERA_ZOOM:
				float zoomval = (float)commandArg.Arguments [0];
				Zoom (zoomval);
				break;
			case COMMAND_TYPE.RESET_CAMERA:
				ResetCamera ();
				break;
			case COMMAND_TYPE.CAMERA_ROTATION:
				Quaternion newRotation = (Quaternion)commandArg.Arguments [0];
				RotateCamera (newRotation);
				break;
			case COMMAND_TYPE.CAMERA_CHANGE:
				CamType = (CameraType)(commandArg.Arguments [0]);
				break;
			}
		}
	}
    
	private void RotateCamera ()
	{
		if (_currentCamera != null) {
			Vector2 inputAxes = new Vector2 (_horizontalInput, _verticalInput);
			inputAxes *= CameraRotationSpeed * Time.deltaTime;
			_currentCamera.RotateCamera (inputAxes);
		}
	}

	private void RotateCamera (Quaternion newRotation)
	{
		if (_currentCamera != null)
			_currentCamera.SetOverrideRotation (newRotation);
	}

    
#if UNITY_EDITOR
	private void OnValidate ()
	{
		ChangeCameraType (_camType);
		CameraTargetChanged (_lookAtTarget);
	}
#endif
    
	private void DisableAllCameraTypes ()
	{
		ChaseCam.enabled = false;
		OrbitalCam.enabled = false;
		FPSCam.enabled = false;

	}
    
	private void DisableCurrentCamera ()
	{
		if (_currentCamera != null) {
			_currentCamera.enabled = false;
		}
	}
    
	private void EnableCurrentCamera ()
	{
		if (_currentCamera != null) {
			_currentCamera.enabled = true;
		}
	}
    
	private void CameraTargetChanged (GameObject target)
	{
		if (_currentCamera != null && target != null) {
			if (CamType == CameraType.CHASE_CAM) {
				_currentCamera.TargetNode = target.transform.FindChild ("TPCameraNode");
			}
			if (CamType == CameraType.ORBITAL_CAM || CamType == CameraType.FPS_CAM) {
				_currentCamera.TargetNode = target.GetEstimatedHeadNode ();// GetEstimatedHeadNode(target);//target.transform.GetCenterNode();
			}
			if (CamType == CameraType.OVERHEAD_CAM) {
				_currentCamera.TargetNode = GetOverheadNode (target);
			}
		}
	}
    
	private Transform GetOverheadNode (GameObject target)
	{
		Transform overheadNode = target.transform.Find ("OverheadNode");
		if (overheadNode == null) {
			Bounds calcBounds = new Bounds (target.transform.position, Vector3.zero);
			Renderer[] renders = target.GetComponentsInChildren<Renderer> ();
			foreach (Renderer render in renders) {
				calcBounds.Encapsulate (render.bounds);
			}
			overheadNode = new GameObject ("OverheadNode").transform;
			overheadNode.transform.position = new Vector3 (calcBounds.center.x, calcBounds.max.y + 0.5f, calcBounds.center.z);
			overheadNode.transform.parent = target.transform;
		}
		return overheadNode;
	}

	private Transform GetEstimatedHeadNode (GameObject target)
	{
		Transform headNode = target.transform.Find ("EstimatedHeadNode");
		if (headNode == null) {
			Transform center = target.transform.GetCenterNode ();
			Bounds calcBounds = new Bounds (center.position, Vector3.zero);
			Renderer[] renders = target.GetComponentsInChildren<Renderer> ();
			foreach (Renderer render in renders) {
				calcBounds.Encapsulate (render.bounds);
			}
			headNode = new GameObject ("EstimatedHeadNode").transform;
			headNode.transform.position = center.position + 0.5f * calcBounds.extents.y * Vector3.up;
			headNode.transform.parent = target.transform;
		}
		return headNode;
	}
    
	private void ChangeCameraType (CameraType newCamType)
	{
		//DisableAllCameraTypes();
		DisableCurrentCamera ();
		switch (newCamType) {
		case CameraType.CHASE_CAM:
			_currentCamera = ChaseCam;
                //ChaseCam.enabled = true;
			break;
		case CameraType.ORBITAL_CAM:
                //OrbitalCam.enabled = true;
			_currentCamera = OrbitalCam;
			break;
		case CameraType.OVERHEAD_CAM:
			_currentCamera = OrbitalCam;
			break;
		case CameraType.FPS_CAM:
			_currentCamera = FPSCam;
			break;
		}

		//Cursor.visible = newCamType != CameraType.OVERHEAD_CAM;
		//Cursor.lockState = (newCamType == CameraType.OVERHEAD_CAM) ? CursorLockMode.Locked : CursorLockMode.Confined;

		EnableCurrentCamera ();
		CameraTargetChanged (LookAtTarget);
	}
    
	// Use this for initialization
	void Start ()
	{
		ChangeCameraType (CamType);
		OnCameraTypeChanged += ChangeCameraType;
		OnCameraTargetChanged += CameraTargetChanged;
	}
	
	// Update is called once per frame
	void Update ()
	{
		RotateCamera ();
	}
    
	private void OnDestroy ()
	{
		OnCameraTypeChanged = null;
		OnCameraTargetChanged = null;
		foreach (IController controller in _controllerList) {
			controller.OnControllerCommandsFired -= OnControlCommand;
		}
		_controllerList.Clear ();
	}
    #endregion
}
