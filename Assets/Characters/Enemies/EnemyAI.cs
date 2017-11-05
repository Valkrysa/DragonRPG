using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	[RequireComponent(typeof(WeaponSystem))]
	public class EnemyAI : MonoBehaviour {
		[SerializeField] private float chaseRadius = 10f;

		private ChatReaction chatReaction = null;
		private PlayerControl playerControl;
		private bool isAttacking = false;
		private float attackRange;

		private void Start() {
			playerControl = FindObjectOfType<PlayerControl>();
			chatReaction = GetComponent<ChatReaction>();
		}

		private void Update() {
			WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
			attackRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

			float distanceToTarget = Vector3.Distance(playerControl.transform.position, transform.position);

			/*
			if (chatReaction) {
				chatReaction.React();
			}
			*/
		}

		private void OnDrawGizmos() {
			// draw attack sphere
			Gizmos.color = new Color(255f, 0f, 0f, .5f);
			Gizmos.DrawWireSphere(transform.position, attackRange);

			// draw move sphere
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, chaseRadius);
		}
	}
}
