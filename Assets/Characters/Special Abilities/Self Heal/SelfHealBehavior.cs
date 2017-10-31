using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class SelfHealBehavior : MonoBehaviour, ISpecialAbility {

		private SelfHealConfig config = null;
		private Player player = null ;
		private AudioSource playerAudioSource = null;

		// Use this for initialization
		void Start() {
			player = GetComponent<Player>();
			playerAudioSource = player.GetComponent<AudioSource>();
		}

		// Update is called once per frame
		void Update() {

		}

		public void SetConfig(SelfHealConfig configToSet) {
			config = configToSet;
		}

		public void Use(AbilityUseParams abilityUseParams) {
			Heal(abilityUseParams);
			PlayParticleEffect();

			playerAudioSource.clip = config.GetAudioClip();
			playerAudioSource.Play();
		}

		private void Heal(AbilityUseParams abilityUseParams) {
			player.Heal(config.GetExtraHealth());

		}

		private void PlayParticleEffect() {
			GameObject particlePrefab = config.GetParticlePrefab();
			GameObject particlePrefabToUse = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
			particlePrefabToUse.transform.SetParent(transform);
			ParticleSystem myParticleSystem = particlePrefabToUse.GetComponent<ParticleSystem>();
			myParticleSystem.Play();
			Destroy(particlePrefabToUse, myParticleSystem.main.duration);
		}
	}
}
