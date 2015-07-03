using UnityEngine;
using System.Collections;

namespace WP.Character
{
	public class CharacterBaseState
	{
		public virtual void OnEnter (CharacterStateMachine csm)
		{
		}

		public virtual void Update (CharacterStateMachine csm, float deltaTime = 0f)
		{
			if (deltaTime == 0f) {
				deltaTime = Time.deltaTime;
			}
		}

		public virtual void OnExit (CharacterStateMachine csm)
		{
		}

	}
}