using UnityEngine;
using System.Collections;

public class OrbitalCamera : BaseCamera
{
	public float Distance = 10f;
	public float MinDistance = 3f;
	public float MaxDistance = 15f;
	public float DistanceSmooth = 0.05f;
	private float _velDistance = 0f;
	public float SmoothingPower = 0.35f;
    
	public float ZoomFactor = 5f;
    
	private float _scrollDeadZone = 0.01f;
    
	private float _targetDistance = 0;
    
	private float _upperBound_Y = 60f;
	private float _lowerBound_Y = -60f;
    
	private Vector2 _currentEulerXY = new Vector2 (0f, 90f);
	private Vector2 _prevEulerXY = new Vector2 (0f, 0f);
	//private Vector3 _smoothVel = new Vector3(0, 0, 0);
    
	private void OnValidate ()
	{
		_targetDistance = Distance;
		ValidateDistance ();
	}
	
	protected override void Start ()
	{
		_targetDistance = Distance;
	}
    
	private void OnEnable ()
	{
		_currentEulerXY.y = transform.rotation.eulerAngles.x;
		_currentEulerXY.x = transform.rotation.eulerAngles.y;
		_prevEulerXY = _currentEulerXY;
	}
    
	// Update is called once per frame
	protected override void Update ()
	{
#if UNITY_EDITOR
		//HandleMouseInput();
#endif
		ValidateDistance ();
		CalculateDesiredDistance ();
		UpdatePosition ();
	}
    
	private void HandleMouseInput ()
	{
		float tempMouseScroll = Input.GetAxis ("Mouse ScrollWheel");
		if (Mathf.Abs (tempMouseScroll) > _scrollDeadZone) {
			Zoom (tempMouseScroll);
		}
        
	}
    
	public override void Zoom (float delta)
	{
		_targetDistance = Distance - delta * ZoomFactor;
	}
    
	private void ValidateDistance ()
	{
		_targetDistance = Mathf.Clamp (_targetDistance, MinDistance, MaxDistance);
		Distance = Mathf.Clamp (Distance, MinDistance, MaxDistance);
	}
    
	private void CalculateDesiredDistance ()
	{
		if (Distance != _targetDistance) {
			Distance = Mathf.SmoothDamp (Distance, _targetDistance, ref _velDistance, DistanceSmooth);
		}
	}
    
	private void UpdatePosition ()
	{
		if (TargetNode != null) {
			Vector3 direction = new Vector3 (0, 0, -Distance);
			//Quaternion rotation = Quaternion.Euler(new Vector3(_mouse_Y, _mouse_X, 0));
			float eulerX = Mathf.Lerp (_prevEulerXY.y, _currentEulerXY.y, Mathf.Pow (Time.smoothDeltaTime, SmoothingPower));
			Quaternion rotation = Quaternion.Euler (new Vector3 (eulerX, _currentEulerXY.x, 0));
			RaycastHit hitInfo;
			Ray testRay = new Ray (TargetNode.position, rotation * direction);
			if (Physics.Raycast (testRay, out hitInfo, Distance)) {
				direction.z = -hitInfo.distance;
				Distance = hitInfo.distance;
			}
            
			//transform.position = Vector3.SmoothDamp(transform.position, TargetNode.position + rotation * direction, ref _smoothVel, Time.smoothDeltaTime);
			transform.position = TargetNode.position + rotation * direction;//Vector3.Lerp(transform.position, TargetNode.position + rotation * direction, Mathf.Pow(Time.deltaTime, SmoothingPower));
			transform.LookAt (TargetNode.position);
			_prevEulerXY = _currentEulerXY;
			//transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Mathf.Pow(Time.deltaTime, SmoothingPower));
		}
	}
    
	public override void RotateCamera (Vector2 inputAxis)
	{
		_currentEulerXY.x = CheckEulerAngleRange (_currentEulerXY.x + inputAxis.x);
		_currentEulerXY.y = CheckEulerAngleRange (_currentEulerXY.y + inputAxis.y);
		_currentEulerXY.y = Mathf.Clamp (_currentEulerXY.y, _lowerBound_Y, _upperBound_Y);
	}
    
	public override void ResetCamera ()
	{
		_currentEulerXY.y = 90f;
		//base.ResetCamera();
	}
    
	private float CheckEulerAngleRange (float val)
	{
		return val > 360f ? val - 360f : val < -360f ? val + 360f : val;
	}

	public override void SetOverrideRotation (Quaternion rotation)
	{
		_currentEulerXY.x = rotation.eulerAngles.y;
		_currentEulerXY.y = rotation.eulerAngles.x;
		//base.SetOverrideRotation(rotation);
	}
}
