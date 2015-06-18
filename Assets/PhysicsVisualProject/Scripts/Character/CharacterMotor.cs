using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterMotor : MonoBehaviour
{
	public const float GRAVITY = 10.0f;
	public const float MAX_VELOCITY_CHANGE = 10.0f;

	public enum MotorState
	{
		STOPPED,
		MOVING,
	}

	private ContactPoint[] _contactedPoints = new ContactPoint[0];
    
	public float ForwardSpeed = 1f;
	public float BackSpeed = 0.5f;
	public float SideSpeed = 1f;
	public float JumpPower = 1f;

	private MotorState _motorState = MotorState.STOPPED;
	private float _motorOutput = 0f;
	private float _movementSpeed = 1.0f;
	private Vector3 _moveVector = Vector3.zero;
	private bool _isOnGround = false;
	public bool IsOnGround {
		get {
			return _isOnGround;
		}
	}

	private Rigidbody mRigidbody = null;
	private Collider mCollider = null;

	public bool GravityAffect = true;
    
	private void OnMovementSpeedChanged (object sender, WP.Stat.StatValueEventArgs e)
	{
		_movementSpeed = (float)e.Value;
	}

	void Awake ()
	{
		mRigidbody = this.GetComponent<Rigidbody> ();
		mCollider = this.GetComponent<Collider> ();
	}
	
	// Use this for initialization
	void Start ()
	{
		mRigidbody.freezeRotation = true;
		mRigidbody.useGravity = false;
		mRigidbody.mass = 50f;
		CharacterStats stats = GetComponent<CharacterStats> ();
		stats.RegisterStatChangeListener ("MovementSpeed", OnMovementSpeedChanged);
		_movementSpeed = stats.GetStatValue<float> ("MovementSpeed") > 0f ? stats.GetStatValue<float> ("MovementSpeed") : 1.0f;
	}
    
	// Update is called once per frame
	void Update ()
	{
		QueryOnGround ();
		switch (_motorState) {
		case MotorState.MOVING:
			_motorOutput = Mathf.Lerp (_motorOutput, 1f, Time.deltaTime);
			break;
		case MotorState.STOPPED:
			_motorOutput = Mathf.Lerp (_motorOutput, 0f, 0.99f);
			break;
		}
	}
    
	public void MoveForward (float speed = 1f)
	{
//        Vector3 currentMoveVectorComparison = Vector3.Project(_moveVector, transform.TransformDirection(Vector3.forward));
//        float comparisonVal = ForwardSpeed * speed;
		_moveVector += transform.TransformDirection (Vector3.forward) * ForwardSpeed * speed;
		_motorState = MotorState.MOVING;
	}
    
	public void MoveBack (float speed = 1f)
	{
		_moveVector += transform.TransformDirection (Vector3.back) * BackSpeed * speed;
		_motorState = MotorState.MOVING;
	}
    
	public void MoveLeft (float speed = 1f)
	{
		_moveVector += transform.TransformDirection (Vector3.left) * SideSpeed * speed;
		_motorState = MotorState.MOVING;
	}
    
	public void MoveRight (float speed = 1f)
	{
		_moveVector += transform.TransformDirection (Vector3.right) * SideSpeed * speed;
		_motorState = MotorState.MOVING;
	}
    
	public void ThrottleForward (float throttle = 0f)
	{
		_moveVector += transform.TransformDirection (Vector3.forward) * throttle * ((throttle > 0) ? ForwardSpeed : BackSpeed);
		_motorState = MotorState.MOVING;
	}
    
	public void ThrottleSideways (float throttle = 0f)
	{
		_moveVector += transform.TransformDirection (Vector3.forward) * throttle * SideSpeed;
		_motorState = MotorState.MOVING;
	}
    
	public void ResetMoveVector ()
	{
		_moveVector = Vector3.zero;
		_motorState = MotorState.STOPPED;
		//Brake();
	}
    
	// Useless for now. Meant for accelerating vehicles and such.
	public void Brake ()
	{
		if (Mathf.Abs (GetComponent<Rigidbody> ().velocity.y) < 1E-5f) {
			mRigidbody.velocity = Vector3.zero;
		}
	}
    
	public void Jump ()
	{
		if (_isOnGround) {
			mRigidbody.velocity += JumpPower * Vector3.up;
		}
	}

	private void FixedUpdate ()
	{
		if (_isOnGround || !GravityAffect) {
			Vector3 targetVelocity = (_moveVector * _movementSpeed);
			mRigidbody.AddForce (CalculateVelocityChange (targetVelocity), ForceMode.VelocityChange);
		}

		if (GravityAffect)
			mRigidbody.AddForce (0, -GRAVITY * mRigidbody.mass, 0, ForceMode.Force);
		//_isOnGround = false;
	}

	private Vector3 CalculateVelocityChange (Vector3 targetVelocity)
	{
		Vector3 velocityChange = (targetVelocity - GetComponent<Rigidbody> ().velocity);
		velocityChange.y = 0;
		return Vector3.ClampMagnitude (velocityChange, MAX_VELOCITY_CHANGE);
	}

	void OnCollisionStay (Collision coll)
	{
		_contactedPoints = coll.contacts;
		//bool onground = false;
        
		//ContactPoint[] contactPoints = coll.contacts;
		//int contactPointAmt = contactPoints.Length;
		//for (int i = 0; i < contactPointAmt; ++i)
		//{
		//    ContactPoint contact = contactPoints[i];
		//    if (contact.point.y < this.collider.bounds.center.y)
		//    {
		//        onground = true;
		//        break;
		//    }
		//}
		//_isOnGround = onground;
	}

	private void QueryOnGround ()
	{
		bool onground = false;

		int contactPointAmt = _contactedPoints.Length;
		for (int i = 0; i < contactPointAmt; ++i) {
			ContactPoint contact = _contactedPoints [i];
			if (contact.point.y < mCollider.bounds.center.y) {
				onground = true;
				break;
			}
		}
		_isOnGround = onground;
	}
}
