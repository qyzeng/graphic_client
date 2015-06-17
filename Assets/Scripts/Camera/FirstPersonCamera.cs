using UnityEngine;
using System.Collections;

public class FirstPersonCamera : BaseCamera
{
	private Transform _previousTarget = null;
	private bool _TargetChanged = false;

	public delegate void TargetChangedHandler ();
	public event TargetChangedHandler OnTargetChanged;
	
	private Vector2 _currentEulerXY;

	private float _upperBound_Y = 60f;
	private float _lowerBound_Y = -60f;
	public float SmoothingPower = 0.35f;

	protected override void Start ()
	{
		base.Start ();
		this.OnTargetChanged += TargetChangedAction;
		StartCoroutine (CheckTargetChangedEvent ());
	}

	private void TargetChangedAction ()
	{
		if (TargetNode != null) {
			_currentEulerXY.x = TargetNode.rotation.eulerAngles.y;
			_currentEulerXY.y = TargetNode.rotation.eulerAngles.x;
		}
	}

	protected override void Update ()
	{
		CheckTargetChanged ();
		UpdatePosition ();
		UpdateRotation ();
	}

	private void UpdatePosition ()
	{
		if (TargetNode != null) {
			transform.position = TargetNode.position;//Vector3.Lerp (transform.position, TargetNode.position, Mathf.Pow (Time.deltaTime, SmoothingPower));
		}
	}

	private void UpdateRotation ()
	{
		Quaternion targetRot = Quaternion.Euler (_currentEulerXY.y, _currentEulerXY.x, 0);
		transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, Mathf.Pow (Time.deltaTime, SmoothingPower));
	}

	private void CheckTargetChanged ()
	{
		if (_previousTarget != TargetNode) {
			_TargetChanged = true;
			_previousTarget = TargetNode;
		}
	}

	private void OnDestroy ()
	{
		OnTargetChanged = null;
	}

	private IEnumerator CheckTargetChangedEvent ()
	{
		while (true) {
			if (_TargetChanged) {
				_TargetChanged = false;
				if (OnTargetChanged != null) {
					OnTargetChanged ();
				}
			}
			yield return new WaitForSeconds (0.5f);
		}
	}

	public override void RotateCamera (Vector2 inputAxis)
	{
		_currentEulerXY.x = CheckEulerAngleRange (_currentEulerXY.x + inputAxis.x);
		_currentEulerXY.y = CheckEulerAngleRange (_currentEulerXY.y + inputAxis.y);
		//_currentEulerXY.y = Mathf.Clamp (_currentEulerXY.y, _lowerBound_Y, _upperBound_Y);
	}

	public override void SetOverrideRotation (Quaternion rotation)
	{
		_currentEulerXY.x = rotation.eulerAngles.y;
		_currentEulerXY.y = rotation.eulerAngles.x;
		//base.SetOverrideRotation(rotation);
	}

	private float CheckEulerAngleRange (float val)
	{
		return val > 360f ? val - 360f : val < -360f ? val + 360f : val;
	}
}
