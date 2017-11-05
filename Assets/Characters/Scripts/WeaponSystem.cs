using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters {
	public class WeaponSystem : MonoBehaviour {
		[SerializeField] private float baseDamage = 25;
		[SerializeField] private WeaponConfig currentWeaponConfig;

		private Character character;
		private GameObject target;
		private GameObject weaponObject;
		private Animator animator;
		private float lastHitTime;
		private const string ATTACK_TRIGGER = "Attack";
		private const string DEFAULT_ATTACK = "DEFAULT ATTACK";

		// Use this for initialization
		void Start() {
			character = GetComponent<Character>();
			animator = GetComponent<Animator>();

			PutWeaponInHand(currentWeaponConfig);
			SetAttackAnimation();
		}

		// Update is called once per frame
		void Update() {

		}

		public void PutWeaponInHand(WeaponConfig weaponConfig) {
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

		public void AttackTarget(GameObject targetToAttack) {
			target = targetToAttack;
			// repeat attack co-routine
			Debug.Log("Attacking " + targetToAttack.name + "!");
		}

		public WeaponConfig GetCurrentWeapon() {
			return currentWeaponConfig;
		}

		private void ExecuteAttack() {
			Component damageable = target.GetComponent(typeof(HealthSystem));
			if (damageable && (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())) {
				SetAttackAnimation();
				animator.SetTrigger(ATTACK_TRIGGER);
				(damageable as HealthSystem).TakeDamage(CalculateDamage());
				lastHitTime = Time.time;
			}
		}

		private void SetAttackAnimation() {
			AnimatorOverrideController animatorOverrideController = character.GetAnimatorOverrideController();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
		}

		private float CalculateDamage() {
			return baseDamage + currentWeaponConfig.GetAdditionalDamage();
		}
	}
}
