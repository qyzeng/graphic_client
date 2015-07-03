using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WP.Character
{
	public static class CharacterStateUtility
	{
		public static readonly Dictionary<CharacterState, CharacterBaseState> StateReference = new Dictionary<CharacterState, CharacterBaseState> ()
		{
			{ CharacterState.IDLE, new CharacterIdleState() },
			{ CharacterState.MOVE, new CharacterMoveState() },
			{ CharacterState.JUMP, new CharacterJumpState() },
			{ CharacterState.FLY, new CharacterFlyState() },
			{ CharacterState.INTERACT, new CharacterInteractState() },
			{ CharacterState.ACTION, new CharacterActionState() },
			{ CharacterState.DEAD, new CharacterDieState() }
		};
	}
}
