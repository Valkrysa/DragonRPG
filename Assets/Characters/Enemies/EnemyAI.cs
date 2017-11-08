using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	[RequireComponent(typeof(HealthSystem))]
	[RequireComponent(typeof(Character))]
	[RequireComponent(typeof(WeaponSystem))]
	public class EnemyAI : MonoBehaviour {
		private enum State {
			idle, patrolling, chasing, attacking, fleeing, following
		}

		[SerializeField] private WaypointContainer patrolPath;
		[SerializeField] private float waypointPauseTime = 0.5f;
		[SerializeField] private float waypointTolerance = 4;
		[SerializeField] private float chaseRadius = 2f;
		[SerializeField] private bool isFriendly = false;

		private State state = State.idle;
		private HealthSystem health;
		private PlayerControl playerControl;
		private Character character;
		private float attackRange;
		private float distanceToTarget;
		private int nextWaypointIndex = 0;
		private bool hasGreeted = false;

		private void Start() {
			health = GetComponent<HealthSystem>();
			character = GetComponent<Character>();

			playerControl = FindObjectOfType<PlayerControl>();
		}

		private void Update() {
			WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
			attackRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

			distanceToTarget = Vector3.Distance(playerControl.transform.position, transform.position);

			bool inWeaponRange = distanceToTarget <= attackRange;
			bool inChaseRange = distanceToTarget > attackRange && distanceToTarget <= chaseRadius;
			bool outsideChaseRange = distanceToTarget > chaseRadius;

			if (outsideChaseRange && state != State.patrolling) {
				StopAllCoroutines();
				weaponSystem.StopAttacking();
				StartCoroutine(Patrol());
			}
			if (inChaseRange && state != State.chasing) {
				StopAllCoroutines();
				weaponSystem.StopAttacking();
				StartCoroutine(ChasePlayer());
			}
			if (inWeaponRange && state != State.attacking) {
				StopAllCoroutines();
				state = State.attacking;
				weaponSystem.AttackTarget(playerControl.gameObject);
			}
			if (isFriendly && distanceToTarget <= chaseRadius && state != State.following) {
				// follow state // following is moving to a spot right behind the player
			}
			if (health.HealthAsPercentage() <= .05 && state != State.fleeing) {
				// flee state
			}
		}

		private void OnDrawGizmos() {
			// draw attack sphere
			Gizmos.color = new Color(255f, 0f, 0f, .5f);
			Gizmos.DrawWireSphere(transform.position, attackRange);

			// draw move sphere
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, chaseRadius);
		}

		private IEnumerator Patrol() {
			state = State.patrolling;

			while (patrolPath != null) {
				Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
				character.SetDestination(nextWaypointPos);
				CycleWaypointWhenClose(nextWaypointPos);

				yield return new WaitForSeconds(waypointPauseTime);
			}	
		}

		private IEnumerator ChasePlayer() {
			state = State.chasing;

			while (distanceToTarget >= attackRange) {
				character.SetDestination(playerControl.transform.position);
				yield return new WaitForEndOfFrame();
			}
		}

		private void CycleWaypointWhenClose(Vector3 currentWaypointPosition) {
			if (Vector3.Distance(transform.position, currentWaypointPosition) <= waypointTolerance) {
				// I orginally had a if check, but teachers use of modulo is better.
				nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
			}
		}
	}
}
