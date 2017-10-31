﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;
using RPG.Weapon;
using RPG.Core;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace RPG.Characters {
	public class Player : MonoBehaviour, IDamageable {
		
		[SerializeField] private float maxHealthPoints = 100;
		[SerializeField] private float baseDamage = 25;
		[SerializeField] private Weapons weaponInUse;
		[SerializeField] private AnimatorOverrideController animatorOverrideController;
		[SerializeField] private AbilityConfig[] _abilitiesConfig;
		[SerializeField] private AudioClip[] damageSounds;
		[SerializeField] private AudioClip[] deathSounds;
		[SerializeField] [Range(0.1f, 1.0f)] private float criticalHitChancePercent = 0.1f;
		[SerializeField] private float criticalHitMultiplier = 1.25f;
		[SerializeField] private ParticleSystem criticalHitParticle;

		private Enemy currentEnemy = null;
		private AudioSource audioSource = null;
		private CameraRaycaster cameraRaycaster = null;
		private Animator animator = null;
		private Energy energy = null;
		private ChatBox chatBox = null;
		private float currentHealthPoints = 0f;
		private float lastHitTime = 0f;
		private const string DEATH_TRIGGER = "Death";
		private const string ATTACK_TRIGGER = "Attack";

		public float healthAsPercentage {
			get {
				return currentHealthPoints / (float)maxHealthPoints;
			}
		}

		private void Start() {
			animator = GetComponent<Animator>();
			energy = GetComponent<Energy>();
			audioSource = GetComponent<AudioSource>();

			chatBox = FindObjectOfType<ChatBox>();
			chatBox.AddChatEntry("You: I'm almost back to the village.");
			chatBox.AddChatEntry("You: What is that fort doing up ahead?");

			SetCurrentMaxHealth();
			AttachInitialAbilities();
			PutWeaponInHand();
			RegisterForMouseClick();
			OverrideAnimatorController();

		}

		private void Update() {
			if (healthAsPercentage > Mathf.Epsilon) {
				ScanForAbilityKeyDown();
			}
		}

		private void AttachInitialAbilities() {
			for (int abilityIndex = 0; abilityIndex < _abilitiesConfig.Length; abilityIndex++) {
				_abilitiesConfig[abilityIndex].AttachComponentTo(gameObject);
			}
		}

		private void ScanForAbilityKeyDown() {
			for (int keyIndex = 0; keyIndex < _abilitiesConfig.Length; keyIndex++) {
				if (Input.GetKeyDown(keyIndex.ToString())) {
					AttemptSpecialAbility(keyIndex);
				}
			}
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
			this.currentEnemy = enemy;

			if (IsEnemyInRange()) {
				if (Input.GetMouseButton(0)) {
					ExecuteAttack();
				} else if (Input.GetMouseButtonDown(1)) {
					AttemptSpecialAbility(0);
				}
			}
		}

		private bool IsEnemyInRange() {
			GameObject enemy = currentEnemy.gameObject;
			float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
			if (distanceToEnemy > weaponInUse.GetMaxAttackRange()) {
				return false;
			} else {
				return true;
			}
		}

		private void ExecuteAttack() {
			Component damageable = currentEnemy.GetComponent(typeof(IDamageable));
			if (damageable && (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())) {
				animator.SetTrigger(ATTACK_TRIGGER);
				(damageable as IDamageable).TakeDamage(CalculateDamage());
				lastHitTime = Time.time;
			}
		}

		private float CalculateDamage() {
			float damageToDeal = baseDamage + weaponInUse.GetAdditionalDamage();

			float criticalRoll = UnityEngine.Random.Range(0f, 1f);
			bool isCriticalHit = criticalRoll <= criticalHitChancePercent;
			if (isCriticalHit) {
				damageToDeal = damageToDeal * criticalHitMultiplier;
				criticalHitParticle.Play();
			}

			return damageToDeal;
		}

		private void AttemptSpecialAbility(int abilityIndex) {
			float energyCost = _abilitiesConfig[abilityIndex].GetEnergyCost();

			if (energy.IsEnergyAvailable(energyCost)) {
				energy.ExpendEnergy(energyCost);

				var abilityParams = new AbilityUseParams(currentEnemy, baseDamage);

				_abilitiesConfig[abilityIndex].Use(abilityParams);
			}
		}

		public void TakeDamage(float damage) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
			PlayRandomDamageSound();
			
			if (currentHealthPoints <= 0) {
				StartCoroutine(KillPlayer());
			}
		}

		public void Heal(float healingAmount) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints + healingAmount, 0, maxHealthPoints);
		}

		private IEnumerator KillPlayer() {
			float deathLength = PlayRandomDeathSound();
			chatBox.AddChatEntry("You: Arrrgh! ugh.... ....");
			animator.SetTrigger(DEATH_TRIGGER);

			yield return new WaitForSecondsRealtime(deathLength);

			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		private void PlayRandomDamageSound() {
			int randomIndex = UnityEngine.Random.Range(0, damageSounds.Length);
			audioSource.clip = damageSounds[randomIndex];
			audioSource.Play();
		}

		private float PlayRandomDeathSound() {
			int randomIndex = UnityEngine.Random.Range(0, deathSounds.Length);
			audioSource.clip = deathSounds[randomIndex];
			audioSource.Play();

			return deathSounds[randomIndex].length;
		}
	}
}
