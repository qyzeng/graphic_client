using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterMoveState : CharacterBaseState
	{
		public override void OnEnter (CharacterStateMachine csm)
		{
			//base.OnEnter (csm);
			if (csm.ModelObject != null) {
				ModelAudioData audioData = ModelAudioData.GetModelAudioData (csm.ModelObject);
				if (audioData.MovingClip != null) {
					csm.InvokeRepeating ("PlayMoveSound", 0f, audioData.MovingClip.length);
				}
			}
		}

		public override void Update (CharacterStateMachine csm, float deltaTime)
		{
			//base.Update (csm, deltaTime);
			csm.Motor.ResetMoveVector ();
			if (csm.MotionVector.y > 0) {
				if (csm.MotionVector.x > 0) {
					csm.Motor.MoveRight (csm.MotionVector.x);
				} else {
					csm.Motor.MoveLeft (-csm.MotionVector.x);
				}
				csm.Motor.MoveForward (csm.MotionVector.y);
			} else {
				csm.Motor.MoveBack (-csm.MotionVector.y);
			}
			csm.AnimationControl.ForwardSpeed = csm.MotionVector.y;
			csm.AnimationControl.SideSpeed = csm.MotionVector.x;
		}

		public override void OnExit (CharacterStateMachine csm)
		{
			//base.OnExit (csm);
			csm.CancelInvoke ("PlayMoveSound");
		}
	}
}
