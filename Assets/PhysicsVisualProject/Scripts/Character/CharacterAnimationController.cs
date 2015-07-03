using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterAnimationController : BaseObjectAnimator
	{
		private const string ANIMATOR_FORWARD = "Speed";
		private const string ANIMATOR_SIDE = "Direction";
		private const string ANIMATOR_GESTURE = "Gesture";
    
		private float _forwardSpeed = 0;
		private float _sideSpeed = 0;
		private float _movementSpeed = 1f;
    
		public float ForwardSpeed {
			get {
				return _forwardSpeed;
			}
			set {
				if (_forwardSpeed != value) {
					_forwardSpeed = value;
					UpdateForwardSpeed ();
				}
			}
		}
    
		public float SideSpeed {
			get {
				return _sideSpeed;
			}
			set {
				if (_sideSpeed != value) {
					_sideSpeed = value;
					UpdateSideSpeed ();
				}
			}
		} 
	
		// Use this for initialization
		void Start ()
		{
			CharacterStats charStats = gameObject.GetComponent<CharacterStats> ();
			if (charStats != null) {
				charStats.RegisterStatChangeListener ("MovementSpeed", OnSpeedStatsChanged);
				_movementSpeed = charStats.GetStatValue<float> ("MovementSpeed");
//			charStats.RegisterStatChangeListener ("WeaponType", OnWeaponTypeChanged);
//			int weapon = charStats.GetStatValue<int> ("WeaponType");
//			SetInt ("WeaponType", weapon);
			}   
		}

		protected override void Update ()
		{
			base.Update ();
//		if (GetBool ("Action") && CurrentStateHasTag ("Action") && IsCurrentAnimationEnd ()) {
//			SetBool ("Action", false);
//		}
		}

		private void ProcessJumping ()
		{

		}
    
		private void UpdateForwardSpeed ()
		{
			SetFloat (ANIMATOR_FORWARD, ForwardSpeed * _movementSpeed);
		}
    
		private void UpdateSideSpeed ()
		{
			SetFloat (ANIMATOR_SIDE, SideSpeed * _movementSpeed);
		}
    
		private void OnSpeedStatsChanged (object sender, WP.Stat.StatValueEventArgs args)
		{
			_movementSpeed = (float)args.Value;
			UpdateForwardSpeed ();
			UpdateSideSpeed ();
		}

		private void OnWeaponTypeChanged (object sender, WP.Stat.StatValueEventArgs args)
		{
			SetInt ("WeaponType", (int)args.Value);
		}
    
		public void Jump ()
		{
			//SetTrigger ("Jump");
			SetBool ("Jump", true);
		}

		public bool IsJumping ()
		{
			return !IsStateInTransition () && IsInState ("Jump");
		}
    
		public void Land ()
		{
			if (IsInState ("Jump")) {
				//SetTrigger ("Landing");
				SetBool ("Jump", false);
			}
		}

		public void Action ()
		{
			//SetBool ("Action", true);
		}

		public bool IsInAction ()
		{
			//return CurrentStateHasTag("Action");
			return GetBool ("Action");
		}

		public void Die ()
		{
			SetBool ("Action", false);
			SetBool ("Die", true);
		}

		public void SetGesture (CharacterAnimUtility.GestureEnum gesture)
		{
			SetInt (ANIMATOR_GESTURE, (int)gesture);
		}
	}
}