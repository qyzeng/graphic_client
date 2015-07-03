using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterInteractState : CharacterBaseState
	{
		public override void OnEnter (CharacterStateMachine csm)
		{
			//base.OnEnter (csm);
			csm.AnimationControl.ForwardSpeed = 0;
			csm.AnimationControl.SideSpeed = 0;
			csm.Motor.ResetMoveVector ();
		}
	}
}
