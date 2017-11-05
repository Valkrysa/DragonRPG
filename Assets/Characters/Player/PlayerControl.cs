using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;
using RPG.Core;
using UnityEngine.UI;
using System;

namespace RPG.Characters {
	public class PlayerControl : MonoBehaviour {
		[SerializeField] private float baseDamage = 25;
		[SerializeField] private Weapons currentWeaponConfig;
		[SerializeField] private AnimatorOverrideController animatorOverrideController;
		[SerializeField] [Range(0.1f, 1.0f)] private float criticalHitChancePercent = 0.1f;
		[SerializeField] private float criticalHitMultiplier = 1.25f;
		[SerializeField] private ParticleSystem criticalHitParticle;

		private Character character;
		private SpecialAbilities abilities;
		private GameObject weaponObject;
		private Enemy currentEnemy;
		private CameraRaycaster cameraRaycaster;
		private Animator animator;
		private HealthSystem health;
		private ChatBox chatBox;
		private float lastHitTime = 0f;
		private const string ATTACK_TRIGGER = "Attack";
		private const string DEFAULT_ATTACK = "DEFAULT ATTACK";
		
		private void Start() {
			character = GetComponent<Character>();
			animator = GetComponent<Animator>();
			abilities = GetComponent<SpecialAbilities>();
			health = GetComponent<HealthSystem>();

			chatBox = FindObjectOfType<ChatBox>();
			chatBox.AddChatEntry("You: I'm almost back to the village.");
			chatBox.AddChatEntry("You: What is that fort doing up ahead?");
			
			PutWeaponInHand(currentWeaponConfig);
			RegisterForMouseEvent();
			SetAttackAnimation();

		}

		private void Update() {
			ScanForAbilityKeyDown();
		}

		public void PutWeaponInHand(Weapons weaponConfig) {
			currentWeaponConfig = weaponConfig;

			GameObject weaponPrefab = weaponConfig.GetWeaponPrefab();
			GameObject dominantHand = RequestDominantHand();

			Destroy(weaponObject);

			weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
			weaponObject.transform.localPosition = currentWeaponConfig.grip.localPosition;
			weaponObject.transform.localRotation = currentWeaponConfig.grip.localRotation;
		}

		public GameObject RequestDominantHand() {
			DominantHand[] dominantHands = GetComponentsInChildren<DominantHand>();
			int dominantHandsFound = dominantHands.Length;

			Assert.AreNotEqual(dominantHandsFound, 0, "No dominant hand found on player. It is required.");
			Assert.IsFalse(dominantHandsFound > 1, "PlayerControl should only have one dominant hand.");

			return dominantHands[0].gameObject;
		}

		private void ScanForAbilityKeyDown() {
			for (int keyIndex = 0; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++) {
				if (Input.GetKeyDown(keyIndex.ToString())) {
					abilities.AttemptSpecialAbility(keyIndex);
				}
			}
		}
		
		private void SetAttackAnimation() {
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
		}

		private void RegisterForMouseEvent() {
			cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
			cameraRaycaster.onMouseOverPossiblyWalkable += OnMouseOverPossiblyWalkable;
		}

		private void OnMouseOverEnemy(Enemy enemy) {
			this.currentEnemy = enemy;

			if (IsEnemyInRange()) {
				if (Input.GetMouseButton(0)) {
					ExecuteAttack();
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
			GameObject enemy = currentEnemy.gameObject;
			float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
			if (distanceToEnemy > currentWeaponConfig.GetMaxAttackRange()) {
				return false;
			} else {
				return true;
			}
		}

		private void ExecuteAttack() {
			Component damageable = currentEnemy.GetComponent(typeof(HealthSystem));
			if (damageable && (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())) {
				SetAttackAnimation();
				animator.SetTrigger(ATTACK_TRIGGER);
				(damageable as HealthSystem).TakeDamage(CalculateDamage());
				lastHitTime = Time.time;
			}
		}

		private float CalculateDamage() {
			float damageToDeal = baseDamage + currentWeaponConfig.GetAdditionalDamage();

			float criticalRoll = UnityEngine.Random.Range(0f, 1f);
			bool isCriticalHit = criticalRoll <= criticalHitChancePercent;
			if (isCriticalHit) {
				damageToDeal = damageToDeal * criticalHitMultiplier;
				criticalHitParticle.Play();
			}

			return damageToDeal;
		}
	}
}
