using System.Collections;
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
		
		void Start() {
			character = GetComponent<Character>();
			animator = GetComponent<Animator>();

			PutWeaponInHand(currentWeaponConfig);
			SetAttackAnimation();
		}
		
		void Update() {
			bool targetIsDead;
			bool targetOutOfRange;

			if (target == null) {
				targetIsDead = false;
				targetOutOfRange = false;
			} else {
				targetIsDead = target.GetComponent<HealthSystem>().HealthAsPercentage() <= Mathf.Epsilon;
				targetOutOfRange = currentWeaponConfig.GetMaxAttackRange() < Vector3.Distance(transform.position, target.transform.position);
			}

			bool characterIsDead = GetComponent<HealthSystem>().HealthAsPercentage() <= Mathf.Epsilon;
			
			if (characterIsDead || targetIsDead || targetOutOfRange) {
				StopAllCoroutines();
			}
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

		public WeaponConfig GetCurrentWeapon() {
			return currentWeaponConfig;
		}

		public void StopAttacking() {
			animator.StopPlayback();
			StopAllCoroutines();
		}

		public void AttackTarget(GameObject targetToAttack) {
			target = targetToAttack;
			StartCoroutine(AttackTargetRepeatedly());
		}

		private IEnumerator AttackTargetRepeatedly() {
			bool attackerStillAlive = GetComponent<HealthSystem>().HealthAsPercentage() >= Mathf.Epsilon;
			bool targetStillAlive = target.GetComponent<HealthSystem>().HealthAsPercentage() >= Mathf.Epsilon;

			while (attackerStillAlive && targetStillAlive) {
				AnimationClip animationClip = currentWeaponConfig.GetAttackAnimClip();
				float animationClipTime = animationClip.length / character.GetAnimationSpeedMultiplier();
				float attackTime = animationClipTime + currentWeaponConfig.GetWaitTimeBetweenAnimations();

				bool timeToAttack = (Time.time - lastHitTime) > attackTime;

				if (timeToAttack) {
					lastHitTime = Time.time;

					AttackTargetOnce();
				}

				yield return new WaitForSeconds(attackTime);
			}
		}

		private void AttackTargetOnce() {
			transform.LookAt(target.transform);
			float damageTiming = currentWeaponConfig.GetDamageDelay();
			SetAttackAnimation();
			animator.SetTrigger(ATTACK_TRIGGER);
			StartCoroutine(DamageAfterDelay(damageTiming));

		}

		private IEnumerator DamageAfterDelay(float delay) {
			yield return new WaitForSeconds(delay);
			if (target != null) {
				target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
			}
		}

		private void SetAttackAnimation() {
			if (!character.GetAnimatorOverrideController()) {
				Debug.Break();
				Debug.LogAssertion("Please provide animator override controller");
			} else {
				AnimatorOverrideController animatorOverrideController = character.GetAnimatorOverrideController();
				animator.runtimeAnimatorController = animatorOverrideController;
				animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
			}
		}

		private float CalculateDamage() {
			return baseDamage + currentWeaponConfig.GetAdditionalDamage();
		}
	}
}
