using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterDieState : CharacterBaseState
	{
		public override void OnEnter (CharacterStateMachine csm)
		{
			csm.AnimationControl.ForwardSpeed = 0;
			csm.AnimationControl.SideSpeed = 0;
			csm.Motor.ResetMoveVector ();
			csm.AnimationControl.Die ();
			csm.Invoke ("PlayDieSound", 0f);
		}
	}
}