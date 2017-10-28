using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;
using RPG.Weapon;
using RPG.Core;

namespace RPG.Characters {
	public class Player : MonoBehaviour, IDamageable {

		[SerializeField] private float maxHealthPoints = 100;
		[SerializeField] private float damagePerHit = 25;
		
		[SerializeField] private Weapons weaponInUse;
		[SerializeField] private AnimatorOverrideController animatorOverrideController;

		private float currentHealthPoints;
		private CameraRaycaster cameraRaycaster;
		private float lastHitTime = 0f;
		private Animator animator;

		public float healthAsPercentage {
			get {
				return currentHealthPoints / (float)maxHealthPoints;
			}
		}

		private void Start() {
			animator = GetComponent<Animator>();

			SetCurrentMaxHealth();

			PutWeaponInHand();
			RegisterForMouseClick();
			OverrideAnimatorController();
		}

		private void SetCurrentMaxHealth() {
			currentHealthPoints = maxHealthPoints;
		}

		private void OverrideAnimatorController() {
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAttackAnimClip();
		}

		private void PutWeaponInHand() {
			GameObject weaponPrefab = weaponInUse.GetWeaponPrefab();

			GameObject dominantHand = RequestDominantHand();

			GameObject weapon = Instantiate(weaponPrefab, dominantHand.transform);
			weapon.transform.localPosition = weaponInUse.grip.localPosition;
			weapon.transform.localRotation = weaponInUse.grip.localRotation;
		}

		public GameObject RequestDominantHand() {
			DominantHand[] dominantHands = GetComponentsInChildren<DominantHand>();
			int dominantHandsFound = dominantHands.Length;

			Assert.AreNotEqual(dominantHandsFound, 0, "No dominant hand found on player. It is required.");
			Assert.IsFalse(dominantHandsFound > 1, "Player should only have one dominant hand.");

			return dominantHands[0].gameObject;
		}

		private void RegisterForMouseClick() {
			cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
		}

		private void OnMouseOverEnemy(Enemy enemy) {
			if (Input.GetMouseButton(0) && IsEnemyInRange(enemy.gameObject)) {
				ExecuteAttack(enemy);
			}
		}

		private bool IsEnemyInRange(GameObject enemy) {
			float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
			if (distanceToEnemy > weaponInUse.GetMaxAttackRange()) {
				return false;
			} else {
				return true;
			}
		}

		private void ExecuteAttack(Enemy enemy) {
			Component damageable = enemy.GetComponent(typeof(IDamageable));
			if (damageable && (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())) {
				animator.SetTrigger("Attack");
				(damageable as IDamageable).TakeDamage(damagePerHit);
				lastHitTime = Time.time;
			}
		}

		public void TakeDamage(float damage) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
		}
	}
}
