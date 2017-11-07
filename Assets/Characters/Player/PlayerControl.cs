using System.Collections;
using UnityEngine;
using RPG.CameraUI;

namespace RPG.Characters {
	public class PlayerControl : MonoBehaviour {
		private WeaponSystem weaponSystem;
		private Character character;
		private SpecialAbilities abilities;
		private ChatBox chatBox;
		
		private void Start() {
			weaponSystem = GetComponent<WeaponSystem>();
			character = GetComponent<Character>();
			abilities = GetComponent<SpecialAbilities>();

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

		private bool IsEnemyInRange(GameObject enemy) {
			float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
			if (distanceToEnemy > weaponSystem.GetCurrentWeapon().GetMaxAttackRange()) {
				return false;
			} else {
				return true;
			}
		}

		private void RegisterForMouseEvent() {
			CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
			cameraRaycaster.onMouseOverPossiblyWalkable += OnMouseOverPossiblyWalkable;
		}

		private void OnMouseOverPossiblyWalkable(Vector3 destination) {
			if (Input.GetMouseButton(0)) {
				weaponSystem.StopAttacking();
				character.SetDestination(destination);
			}
		}

		private void OnMouseOverEnemy(EnemyAI enemyAi) {
			if (Input.GetMouseButton(0) && IsEnemyInRange(enemyAi.gameObject)) {
				weaponSystem.AttackTarget(enemyAi.gameObject);
			} else if (Input.GetMouseButton(0) && !IsEnemyInRange(enemyAi.gameObject)) {
				StartCoroutine(MoveAndAttack(enemyAi.gameObject));
			} else if (Input.GetMouseButtonDown(1) && IsEnemyInRange(enemyAi.gameObject)) {
				abilities.AttemptSpecialAbility(0, enemyAi.gameObject);
			} else if (Input.GetMouseButtonDown(1) && !IsEnemyInRange(enemyAi.gameObject)) {
				StartCoroutine(MoveAndSpecialAbility(0, enemyAi.gameObject));
			}
		}

		private IEnumerator MoveToTarget(GameObject enemy) {
			character.SetDestination(enemy.transform.position);

			while (!IsEnemyInRange(enemy)) {
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator MoveAndAttack(GameObject enemy) {
			yield return StartCoroutine(MoveToTarget(enemy));

			weaponSystem.AttackTarget(enemy);
		}

		private IEnumerator MoveAndSpecialAbility(int specialAbilityIndex, GameObject enemy) {
			yield return StartCoroutine(MoveToTarget(enemy));

			abilities.AttemptSpecialAbility(specialAbilityIndex, enemy);
		}
	}
}
