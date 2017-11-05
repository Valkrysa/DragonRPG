using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.CameraUI;
using RPG.Core;
using UnityEngine.UI;
using System;

namespace RPG.Characters {
	public class PlayerControl : MonoBehaviour {
		[SerializeField] [Range(0.1f, 1.0f)] private float criticalHitChancePercent = 0.1f;
		[SerializeField] private float criticalHitMultiplier = 1.25f;
		[SerializeField] private ParticleSystem criticalHitParticle;

		private WeaponSystem weaponSystem;
		private Character character;
		private SpecialAbilities abilities;
		private EnemyAI currentEnemyAi;
		private CameraRaycaster cameraRaycaster;
		private HealthSystem health;
		private ChatBox chatBox;
		
		private void Start() {
			weaponSystem = GetComponent<WeaponSystem>();
			character = GetComponent<Character>();
			abilities = GetComponent<SpecialAbilities>();
			health = GetComponent<HealthSystem>();

			chatBox = FindObjectOfType<ChatBox>();
			chatBox.AddChatEntry("You: I'm almost back to the village.");
			chatBox.AddChatEntry("You: What is that fort doing up ahead?");
			
			RegisterForMouseEvent();

		}

		private void Update() {
			ScanForAbilityKeyDown();
		}
		
		private void ScanForAbilityKeyDown() {
			for (int keyIndex = 0; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++) {
				if (Input.GetKeyDown(keyIndex.ToString())) {
					abilities.AttemptSpecialAbility(keyIndex);
				}
			}
		}

		private void RegisterForMouseEvent() {
			cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
			cameraRaycaster.onMouseOverPossiblyWalkable += OnMouseOverPossiblyWalkable;
		}

		private void OnMouseOverEnemy(EnemyAI enemyAi) {
			this.currentEnemyAi = enemyAi;

			if (IsEnemyInRange()) {
				if (Input.GetMouseButton(0)) {
					weaponSystem.AttackTarget(currentEnemyAi.gameObject);
				} else if (Input.GetMouseButtonDown(1)) {
					abilities.AttemptSpecialAbility(0);
				}
			}
		}

		private void OnMouseOverPossiblyWalkable(Vector3 destination) {
			if (Input.GetMouseButton(0)) {
				character.SetDestination(destination);
			}
		}

		private bool IsEnemyInRange() {
			GameObject enemy = currentEnemyAi.gameObject;
			float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
			if (distanceToEnemy > weaponSystem.GetCurrentWeapon().GetMaxAttackRange()) {
				return false;
			} else {
				return true;
			}
		}
	}
}
