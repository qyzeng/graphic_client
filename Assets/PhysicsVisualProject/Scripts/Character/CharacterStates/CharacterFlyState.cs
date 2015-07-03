using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterFlyState : CharacterBaseState
	{
		public override void OnEnter (CharacterStateMachine csm)
		{
			csm.Motor.GravityAffect = false;
			csm.AnimationControl.ForwardSpeed = 0;
			csm.Motor.ResetMoveVector ();
		}

		public override void Update (CharacterStateMachine csm, float deltaTime)
		{
			csm.Motor.ResetMoveVector ();
			
			if (csm.MotionVector.x > 0) {
				csm.Motor.MoveRight (csm.MotionVector.x);
			} else {
				csm.Motor.MoveLeft (-csm.MotionVector.x);
			}
			if (csm.MotionVector.y > 0) {
				csm.Motor.MoveForward (csm.MotionVector.y);
			} else {
				csm.Motor.MoveBack (-csm.MotionVector.y);
			}
		}

		public override void OnExit (CharacterStateMachine csm)
		{
			csm.Motor.GravityAffect = true;
		}
	}
}
