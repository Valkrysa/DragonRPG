using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;
using RPG.Weapon;
using RPG.Core;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPG.Characters {
	public class Player : MonoBehaviour, IDamageable {
		
		[SerializeField] private float maxHealthPoints = 100;
		[SerializeField] private float baseDamage = 25;
		[SerializeField] private Weapons weaponInUse;
		[SerializeField] private AnimatorOverrideController animatorOverrideController;
		[SerializeField] private SpecialAbility[] abilities;
		[SerializeField] private AudioClip[] damageSounds;
		[SerializeField] private AudioClip[] deathSounds;

		private AudioSource audioSource;
		private float currentHealthPoints;
		private CameraRaycaster cameraRaycaster;
		private float lastHitTime = 0f;
		private Animator animator;
		private Energy energy;
		private ChatBox chatBox = null;
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
			PutWeaponInHand();
			RegisterForMouseClick();
			OverrideAnimatorController();

			abilities[0].AttachComponentTo(gameObject);
			abilities[1].AttachComponentTo(gameObject);
			abilities[2].AttachComponentTo(gameObject);
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
			if (IsEnemyInRange(enemy.gameObject)) {
				if (Input.GetMouseButton(0)) {
					ExecuteAttack(enemy);
				} else if (Input.GetMouseButtonDown(1)) {
					AttemptSpecialAbility(0, enemy);
				} else if (Input.GetKeyDown(KeyCode.Alpha1)) {
					AttemptSpecialAbility(1, enemy);
				} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
					AttemptSpecialAbility(2, enemy);
				}
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
				animator.SetTrigger(ATTACK_TRIGGER);
				(damageable as IDamageable).AdjustHealth(baseDamage);
				lastHitTime = Time.time;
			}
		}

		private void AttemptSpecialAbility(int abilityIndex, Enemy enemy) {
			float energyCost = abilities[abilityIndex].GetEnergyCost();

			if (energy.IsEnergyAvailable(energyCost)) {
				energy.ExpendEnergy(energyCost);

				var abilityParams = new AbilityUseParams(enemy, baseDamage);

				abilities[abilityIndex].Use(abilityParams);
			}
		}

		public void AdjustHealth(float damage) {
			DamageHealth(damage);

			bool isPlayerDieing = (currentHealthPoints <= 0);
			if (isPlayerDieing) {
				StartCoroutine(KillPlayer());
			}
		}

		private void DamageHealth(float damage) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
			PlayRandomDamageSound();
		}

		private IEnumerator KillPlayer() {
			float deathLength = PlayRandomDeathSound();
			chatBox.AddChatEntry("You: Arrrgh! ugh.... ....");
			animator.SetTrigger(DEATH_TRIGGER);

			yield return new WaitForSecondsRealtime(deathLength);

			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		private void PlayRandomDamageSound() {
			int randomIndex = Random.Range(0, damageSounds.Length);
			audioSource.clip = damageSounds[randomIndex];
			audioSource.Play();
		}

		private float PlayRandomDeathSound() {
			int randomIndex = Random.Range(0, deathSounds.Length);
			audioSource.clip = deathSounds[randomIndex];
			audioSource.Play();

			return deathSounds[randomIndex].length;
		}
	}
}
