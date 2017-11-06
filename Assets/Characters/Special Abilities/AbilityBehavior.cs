using System.Collections;
using UnityEngine;

namespace RPG.Characters {
	public abstract class AbilityBehavior : MonoBehaviour {
		protected AbilityConfig config;

		private const string ATTACK_TRIGGER = "Attack";
		private const string DEFAULT_ATTACK = "DEFAULT ATTACK";
		private const float PARTICLE_CLEAN_UP_DELAY = 20f;

		public abstract void Use(GameObject target);

		public void SetConfig(AbilityConfig configToSet) {
			config = configToSet;
		}

		protected void PlayAbilityAnimation() {
			Character character = GetComponent<Character>();
			Animator animator = GetComponent<Animator>();
			AnimatorOverrideController animatorOverrideController = character.GetAnimatorOverrideController();

			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController[DEFAULT_ATTACK] = config.GetAbilityAnimation();
			animator.SetTrigger(ATTACK_TRIGGER);
		}

		protected void PlayAbilitySound() {
			AudioSource playerAudioSource = GetComponent<AudioSource>();
			AudioClip abilitySound = config.GetRandomAbilitySound();
			playerAudioSource.PlayOneShot(abilitySound);
		}

		protected void PlayParticleEffect() {
			GameObject particlePrefab = config.GetParticlePrefab();
			GameObject particleObject = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
			particleObject.transform.SetParent(transform);
			particleObject.GetComponent<ParticleSystem>().Play();

			StartCoroutine(DestroyParticleWhenFinished(particleObject));
		}

		private IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab) {
			while (particlePrefab.GetComponent<ParticleSystem>().isPlaying) {
				yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
			}
			Destroy(particlePrefab);

			yield return new WaitForEndOfFrame();
		}
	}
}
