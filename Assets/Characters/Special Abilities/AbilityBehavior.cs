using System.Collections;
using UnityEngine;

namespace RPG.Characters {
	public abstract class AbilityBehavior : MonoBehaviour {
		protected AbilityConfig config;

		private const float PARTICLE_CLEAN_UP_DELAY = 20f;

		public abstract void Use(GameObject target);

		public void SetConfig(AbilityConfig configToSet) {
			config = configToSet;
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
