using UnityEngine;
using System.Collections;
using WP.Controller;

namespace WP.Character
{
	[RequireComponent(typeof(CharacterStateMachine))]
	public class CharacterControlStateMachine : MonoBehaviour, IController
	{
		public enum CharacterControlState
		{
			NAN,
			IDLE,
			MOVING,
			ATTACK,
			DIE
		}
    
		private Vector3 _targetPoint = Vector3.zero;
    
		public delegate void StateTransitHandler (CharacterControlState prevState,CharacterControlState newState);
		public event StateTransitHandler StateTransitEvent;
    
		public event ControllerCommandsFireHandler OnControllerCommandsFired;
		private System.Collections.Generic.List<CommandFiredEventArgs> _batchCommands = new System.Collections.Generic.List<CommandFiredEventArgs> ();
    
		internal CharacterStateMachine _csm = null;
		private CharacterStateMachine _characterStateMachine {
			get {
				if (_csm == null) {
					_csm = GetComponent<CharacterStateMachine> ();
				}
				return _csm;
			}
		}
    
		private CharacterControlState _state = CharacterControlState.NAN;
		public CharacterControlState State {
			get {
				return _state;
			}
			set {
				if (_state != value) {
					if (StateTransitEvent != null) {
						StateTransitEvent (_state, value);
					}
					_state = value;
				}
			}
		}
    
		private void OnStateTransit (CharacterControlState prevState, CharacterControlState newState)
		{
			switch (newState) {
			case CharacterControlState.IDLE:
				Idle ();
				break;
			case CharacterControlState.MOVING:
				Moving ();
				break;
			case CharacterControlState.ATTACK:
				Attacking ();
				break;
			case CharacterControlState.DIE:
				Die ();
				break;
			}
		}
    
    #region UNITY METHODS

		void Awake ()
		{
			_characterStateMachine.AddController (this);
			StateTransitEvent += OnStateTransit;
			State = CharacterControlState.IDLE;
		}
	
		// Update is called once per frame
		void Update ()
		{
			DispatchCommands ();
		}
    
		private void OnDestroy ()
		{
			StateTransitEvent = null;
			OnControllerCommandsFired = null;
		}
    #endregion
    
		private void DispatchCommands ()
		{
			if (OnControllerCommandsFired != null) {
				OnControllerCommandsFired (_batchCommands);
			}
			_batchCommands.Clear ();
		}
    
		public void MoveToDestination (Vector3 target)
		{
			_targetPoint = target;
			State = CharacterControlState.MOVING;
			Moving ();
		}
    
		private void Idle ()
		{
			_batchCommands.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_HORIZONTAL, 0f));
			_batchCommands.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_VERTICAL, 0f));
		}
    
		private void Moving ()
		{
			Vector3 targetDirection = _targetPoint - transform.position;

			if (!GetComponent<Collider> ().bounds.Contains (_targetPoint)) {
				Quaternion targetRotation = Quaternion.FromToRotation (Vector3.forward, targetDirection);
				_batchCommands.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_ROTATION, targetRotation));
				_batchCommands.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_VERTICAL, 0.7f));
			} else {
				State = CharacterControlState.IDLE;
			}
		}

		public void Attack ()
		{
			Attacking ();
			State = CharacterControlState.ATTACK;
		}

		private void Attacking ()
		{
			_batchCommands.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_HORIZONTAL, 0f));
			_batchCommands.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_VERTICAL, 0f));
			_batchCommands.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_ACTION));
		}

		public void Die ()
		{

		}
	}
}