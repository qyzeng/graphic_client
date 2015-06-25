using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using WP.Controller;

public enum CharacterState
{
	NAN,
	IDLE,
	MOVE,
	ACTION,
	DEAD,
	INTERACT,
	JUMP,
	STUN,
	FLY,
}

public enum WeaponType
{
	NONE = 0,
	FIST = 1,
	SWORD = 2,
	BOW = 3,
}

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(CharacterAnimationController))]
[RequireComponent(typeof(CharacterLogic))]
public class CharacterStateMachine : MonoBehaviour, IControlListener
{
	public delegate void StateTransitionHandler (CharacterState previousState,CharacterState newState);
	public event StateTransitionHandler OnStateTransit;

	private CharacterMotor _motor;

	private CharacterAnimationController _animationControl;

	private delegate void StateProcessHandler ();
	private StateProcessHandler _CurrentStateProcess;
    
	private Vector2 _motionVector = new Vector2 (0, 0);
	private bool _actionTriggered = false;
	private bool _actionDisabled = false;
	private float _actionCooldown = 0.4f;
	private bool _jumpTriggered = false;
    
	private System.Type _transformType = typeof(Transform);

	private Quaternion _targetRotation = Quaternion.identity;
    
	private bool _motionEnabled {
		get {
			return (CurrentState == CharacterState.IDLE ||
				CurrentState == CharacterState.MOVE);
		}
	}
    
	private System.Collections.Generic.List<IController> _controllerList = new System.Collections.Generic.List<IController> ();

	public GameObject ModelObject = null;

	public CharacterStats Stats {
		get;
		private set;
	}
    
	private CharacterState _currentState = CharacterState.NAN;
	public CharacterState CurrentState {
		get {
			return _currentState;
		}
		private set {
			if (value != _currentState) {
				if (OnStateTransit != null) {
					OnStateTransit (_currentState, value);
				}
				_currentState = value;
			}
		}
	}

	public void RefreshModelObjectData ()
	{
		if (_animationControl != null) {
			_animationControl.ModelObject = ModelObject;
		}
		if (ModelObject != null) {
			Bounds testBounds = new Bounds (this.transform.position, Vector3.zero);
			Renderer[] allRenderers = this.GetComponentsInChildren<Renderer> ();
			foreach (Renderer individualRenderer in allRenderers) {
				testBounds.Encapsulate (individualRenderer.bounds);
			}
			CapsuleCollider collider = this.GetComponent<CapsuleCollider> ();
			collider.center = this.transform.InverseTransformPoint (testBounds.center);
			collider.radius = Mathf.Min (testBounds.extents.x, testBounds.extents.z);
			collider.height = testBounds.size.y;
		}
	}

	void Awake ()
	{
		Stats = GetComponent<CharacterStats> ();
		_motor = GetComponent<CharacterMotor> ();
		_animationControl = GetComponent<CharacterAnimationController> ();
	}

	private void Start ()
	{
		this.OnStateTransit += ProcessStateTransition;
		//_animationControl.ModelObject = ModelObject;
		RefreshModelObjectData ();
		SetState (CharacterState.IDLE);
		_targetRotation = transform.rotation;

		Stats.RegisterStatChangeListener ("ActionSpeed", OnActionSpeedChanged);
		float actionSpeed = Stats.GetStatValue<float> ("ActionSpeed");
		if (actionSpeed > 0f) {
			_actionCooldown = 1f / actionSpeed;
		}
	}

	private void OnActionSpeedChanged (object sender, WP.Stat.StatValueEventArgs args)
	{
		if (args.Id == "ActionSpeed") {
			float newSpeedVal = (float)(args.Value);
			_actionCooldown = 1f / (Mathf.Clamp (newSpeedVal, float.Epsilon, float.MaxValue));
		}
	}
    
	private void ProcessStateTransition (CharacterState prevState, CharacterState newState)
	{
		switch (prevState) {
		case CharacterState.IDLE:
			CancelInvoke ("PlayIdleSound");
			break;
		case CharacterState.MOVE:
			CancelInvoke ("PlayMoveSound");
			break;
		case CharacterState.FLY:
			_motor.GravityAffect = true;
			break;
		}

		switch (newState) {
		case CharacterState.INTERACT:
			Interact ();
			break;
		case CharacterState.ACTION:
			Action ();
			break;
		case CharacterState.DEAD:
                //Destroy(this.gameObject);
			Die ();
			break;
		case CharacterState.MOVE:
			Move ();
			break;
		case CharacterState.STUN:
			Stun ();
			break;
		case CharacterState.FLY:
			Fly ();
			break;
		case CharacterState.JUMP:
			Jump ();
			break;
		default:
			Idle ();
			break;
		}
	}
    
	private void Update ()
	{
		DecideState ();
		ProcessCurrentState ();
		ProcessCurrentRotation ();
	}
    
	private void OnDestroy ()
	{
		OnStateTransit = null;
		foreach (IController controller in _controllerList) {
			controller.OnControllerCommandsFired -= OnControllerCommand;
		}
		_controllerList.Clear ();
	}
    
	private void ProcessCurrentRotation ()
	{
		Vector3 targetEuler = _targetRotation.eulerAngles;
		if (CurrentState != CharacterState.FLY) {
			targetEuler.x = targetEuler.z = 0;
		}
		Quaternion targetRotation = Quaternion.Euler (targetEuler);
		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Mathf.Sqrt (Time.smoothDeltaTime));
	}
    
	private void DecideState ()
	{

		if (_motionEnabled) {
			if (_motionVector.sqrMagnitude > 0) {
				SetState (CharacterState.MOVE);
			} else {
				SetState (CharacterState.IDLE);
			}
		}
		if (_actionTriggered) {
			SetState (CharacterState.ACTION);
		}
		if (_jumpTriggered) {
			_jumpTriggered = false;
			SetState (CharacterState.JUMP);
		}
	}
    
	private void ProcessCurrentState ()
	{
//		switch (CurrentState) {
//		case CharacterState.MOVE:
//			Moving ();
//			break;
//		case CharacterState.IDLE:
//			Idling ();
//			break;
//		case CharacterState.ACTION:
//			InAction ();
//			break;
//		case CharacterState.FLY:
//			Flying ();
//			break;
//		}
		if (_CurrentStateProcess != null) {
			_CurrentStateProcess ();
		}
	}

	private void Jump ()
	{
		_animationControl.Jump ();
		_motor.Jump ();
		_CurrentStateProcess = Jumping;
	}

	private void Jumping ()
	{
		if (_motor.IsOnGround && _animationControl.IsJumping ()) {
			_animationControl.Land ();
			SetState (CharacterState.IDLE);
		}
	}

	private void Fly ()
	{
		_motor.GravityAffect = false;
		_animationControl.ForwardSpeed = 0;
		_motor.ResetMoveVector ();
		_CurrentStateProcess = Flying;
	}

	private void Flying ()
	{
		_motor.ResetMoveVector ();

		if (_motionVector.x > 0) {
			_motor.MoveRight (_motionVector.x);
		} else {
			_motor.MoveLeft (-_motionVector.x);
		}
		if (_motionVector.y > 0) {
			_motor.MoveForward (_motionVector.y);
		} else {
			_motor.MoveBack (-_motionVector.y);
		}
	}

	private void Move ()
	{
		if (ModelObject != null) {
			ModelAudioData audioData = ModelAudioData.GetModelAudioData (ModelObject);
			if (audioData.MovingClip != null) {
				InvokeRepeating ("PlayMoveSound", 0f, audioData.MovingClip.length);
			}
		}
		_CurrentStateProcess = Moving;
	}

	private void PlayMoveSound ()
	{
		if (ModelObject != null) {
			ModelAudioData audioData = ModelAudioData.GetModelAudioData (ModelObject);
			if (audioData.MovingClip != null) {
				AudioSource.PlayClipAtPoint (audioData.MovingClip, transform.position);
			}
		}
	}
    
	private void Moving ()
	{
		_motor.ResetMoveVector ();
		if (_motionVector.y > 0) {
			if (_motionVector.x > 0) {
				_motor.MoveRight (_motionVector.x);
			} else {
				_motor.MoveLeft (-_motionVector.x);
			}
		}
		if (_motionVector.y > 0) {
			_motor.MoveForward (_motionVector.y);
		} else {
			_motor.MoveBack (-_motionVector.y);
		}
		_animationControl.ForwardSpeed = _motionVector.y;
		_animationControl.SideSpeed = _motionVector.x;
	}
    
	private void Idling ()
	{
		_motor.ResetMoveVector ();
	}

	private void Idle ()
	{
		if (ModelObject != null) {
			ModelAudioData audioData = ModelAudioData.GetModelAudioData (ModelObject);
			if (audioData.IdleClip != null) {
				InvokeRepeating ("PlayIdleSound", 0f, audioData.IdleClip.length);
			}
		}
		_animationControl.ForwardSpeed = 0;
		_animationControl.SideSpeed = 0;
		_motor.ResetMoveVector ();  
		_CurrentStateProcess = Idling;
	}

	private void PlayIdleSound ()
	{
		if (ModelObject != null) {
			ModelAudioData audioData = ModelAudioData.GetModelAudioData (ModelObject);
			if (audioData.IdleClip != null) {
				AudioSource.PlayClipAtPoint (audioData.IdleClip, transform.position);
			}
		}
	}
    
	private void Interact ()
	{
		_animationControl.ForwardSpeed = 0;
		_animationControl.SideSpeed = 0;
		_motor.ResetMoveVector ();
	}

	private void Action ()
	{
		_animationControl.ForwardSpeed = 0;
		_animationControl.SideSpeed = 0;
		_motor.ResetMoveVector ();
		_animationControl.Action ();
		_actionTriggered = false;
		PlayActionSound ();
	}

	private void PlayActionSound ()
	{
		if (ModelObject != null) {
			ModelAudioData audioData = ModelAudioData.GetModelAudioData (ModelObject);
			if (audioData.ActionClip != null) {
				AudioSource.PlayClipAtPoint (audioData.ActionClip, transform.position);
			}
		}
	}

	private void InAction ()
	{
		_motor.ResetMoveVector ();
		if (!_animationControl.IsInAction ()) {
			SetState (CharacterState.IDLE);
		}
	}

	private void Die ()
	{
		_animationControl.ForwardSpeed = 0;
		_animationControl.SideSpeed = 0;
		_motor.ResetMoveVector ();
		_animationControl.Die ();
		PlayDieSound ();
	}

	private void PlayDieSound ()
	{
		if (ModelObject != null) {
			ModelAudioData audioData = ModelAudioData.GetModelAudioData (ModelObject);
			if (audioData.DieClip != null) {
				AudioSource.PlayClipAtPoint (audioData.DieClip, transform.position);
			}
		}
	}

	private void Stun ()
	{
		if (ModelObject != null) {
			ModelAudioData audioData = ModelAudioData.GetModelAudioData (ModelObject);
			if (audioData.GetHitClip != null) {
				AudioSource.PlayClipAtPoint (audioData.GetHitClip, transform.position);
			}
		}
	}

	public void SetState (CharacterState newState)
	{
		CurrentState = newState;
	}
    
	public void SetHorizontal (float val)
	{
		_motionVector.x = Mathf.Clamp (val, -1, 1);
	}
    
	public void SetVertical (float val)
	{
		_motionVector.y = Mathf.Clamp (val, -1, 1);
	}

	public void EndInteraction ()
	{
		SetState (CharacterState.IDLE);
        
	}

	private void OnControllerCommand (System.Collections.Generic.List<CommandFiredEventArgs> commands)
	{
		foreach (CommandFiredEventArgs command in commands) {
			switch ((COMMAND_TYPE)(command.Command)) {
			case COMMAND_TYPE.PLAYER_HORIZONTAL:
				SetHorizontal (command.Arguments [0] != null ? (float)(command.Arguments [0]) : 0);
				break;
			case COMMAND_TYPE.PLAYER_VERTICAL:
				SetVertical (command.Arguments [0] != null ? (float)(command.Arguments [0]) : 0);
				break;
			case COMMAND_TYPE.PLAYER_ROTATION:
				_targetRotation = (Quaternion)(command.Arguments [0]);
				break;
			case COMMAND_TYPE.PLAYER_ACTION:
				if (!_actionDisabled) {
					_actionTriggered = true;
					_actionDisabled = true;
					Invoke ("EnableAction", _actionCooldown);
				}
				break;
			case COMMAND_TYPE.PLAYER_JUMP:
				_jumpTriggered = true;
				break;
			}
		}
	}

	private void EnableAction ()
	{
		_actionDisabled = false;
	}
    
	public void AddController (IController controller)
	{
		if (!_controllerList.Contains (controller)) {
			controller.OnControllerCommandsFired += OnControllerCommand;
			_controllerList.Add (controller);
		}
	}
    
	public void RemoveController (IController controller)
	{
		controller.OnControllerCommandsFired -= OnControllerCommand;
		if (_controllerList.Contains (controller)) {
			_controllerList.Remove (controller);
		}
	}

	private void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		Vector3 scale = transform.localScale;
		stream.Serialize (ref position);
		stream.Serialize (ref rotation);
		stream.Serialize (ref scale);
		if (stream.isReading) {
			transform.position = position;
			transform.rotation = rotation;
			_targetRotation = rotation;
			transform.localScale = scale;
		}

		_animationControl.OnSerializeNetworkView (stream, info);
	}
}