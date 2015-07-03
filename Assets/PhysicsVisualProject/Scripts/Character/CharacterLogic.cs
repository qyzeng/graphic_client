using UnityEngine;
using System.Collections;
using WP.Controller;
using WP.Stat;

namespace WP.Character
{
	[RequireComponent(typeof(CharacterStats))]
	public class CharacterLogic : MonoBehaviour, IDamageable, IController
	{
		public event ControllerCommandsFireHandler OnControllerCommandsFired;

		public delegate void DeathHandler (CharacterStats killer);
		public event DeathHandler OnDeath;

		private delegate IEnumerator OnHitHandler ();
		private event OnHitHandler OnHit;

		private CharacterStats _stats;

		public void TakeDamage (GameObject attacker, params StatValueEventArgs[] args)
		{

			foreach (StatValueEventArgs arg in args) {
				_stats.SubtractFromStat (arg.Id, arg.Value);
			}

			if (_stats.GetStatValue ("Health") != null) {
				if (_stats.Compare (_stats.GetStatValue ("Health"), 1) == -1) {
					this.GetComponent<CharacterStateMachine> ().SetState (CharacterState.DEAD);
					CharacterStats attackerStats = attacker.GetComponent<CharacterStats> ();
					if (OnDeath != null)
						OnDeath (attackerStats);
				} else { //if (GameMasterSettings.StunOnHit)
					if (OnHit != null) {
						StartCoroutine (OnHit ());
					}
				}
			}
		}

		void Awake ()
		{
			OnHit += HandleOnHit;
		}

		IEnumerator HandleOnHit ()
		{
			this.GetComponent<CharacterStateMachine> ().SetState (CharacterState.STUN);
			yield return new WaitForSeconds (0.2f);

			if (this.GetComponent<CharacterStateMachine> ().CurrentState != CharacterState.DEAD) {
				this.GetComponent<CharacterStateMachine> ().SetState (CharacterState.IDLE);
			}
			yield return 0;
		}

		// Use this for initialization
		void Start ()
		{
			_stats = GetComponent<CharacterStats> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		private void OnHealthChanged (object sender, StatValueEventArgs arg)
		{
			int health = (int)(arg.Value);
			health = health < 0 ? 0 : health;
			_stats.SetStat ("Health", health);
		}
	}
}