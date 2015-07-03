using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterJumpState : CharacterBaseState
	{
		public override void OnEnter (CharacterStateMachine csm)
		{
			csm.AnimationControl.Jump ();
			csm.Motor.Jump ();
		}

		public override void Update (CharacterStateMachine csm, float deltaTime)
		{
			if (csm.Motor.IsOnGround && csm.AnimationControl.IsJumping ()) {
				csm.AnimationControl.Land ();
				csm.SetState (CharacterState.IDLE);
			}
		}
	}
}
