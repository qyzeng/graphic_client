using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterActionState : CharacterBaseState
	{
		public override void OnEnter (CharacterStateMachine csm)
		{
			csm.AnimationControl.ForwardSpeed = 0;
			csm.AnimationControl.SideSpeed = 0;
			csm.Motor.ResetMoveVector ();
			csm.AnimationControl.Action ();
			csm.Invoke ("PlayActionSound", 0f);
		}

		public override void Update (CharacterStateMachine csm, float deltaTime)
		{
			csm.Motor.ResetMoveVector ();
			if (!csm.AnimationControl.IsInAction ()) {
				csm.RevertToPreviousState ();
			}
		}
	}
}