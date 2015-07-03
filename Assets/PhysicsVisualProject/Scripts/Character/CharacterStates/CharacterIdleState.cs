using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterIdleState : CharacterBaseState
	{
		public override void OnEnter (CharacterStateMachine csm)
		{
			//base.OnEnter (csm);
			if (csm.ModelObject != null) {
				ModelAudioData audioData = ModelAudioData.GetModelAudioData (csm.ModelObject);
				if (audioData.IdleClip != null) {
					csm.InvokeRepeating ("PlayIdleSound", 0f, audioData.IdleClip.length);
				}
			}
			csm.AnimationControl.ForwardSpeed = 0;
			csm.AnimationControl.SideSpeed = 0;
			csm.Motor.ResetMoveVector ();  
		}

		public override void Update (CharacterStateMachine csm, float deltaTime)
		{
			//base.Update (csm, deltaTime);
			csm.Motor.ResetMoveVector ();
		}

		public override void OnExit (CharacterStateMachine csm)
		{
			//base.OnExit (csm);
			csm.CancelInvoke ("PlayIdleSound");
		}
	}
}