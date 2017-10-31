using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehavior : MonoBehaviour, ISpecialAbility {

		private PowerAttackConfig config;
		private Player player = null;
		private AudioSource playerAudioSource = null;

		// Use this for initialization
		void Start() {
			player = GetComponent<Player>();
			playerAudioSource = player.GetComponent<AudioSource>();
		}

		// Update is called once per frame
		void Update() {

		}

		public void SetConfig(PowerAttackConfig configToSet) {
			config = configToSet;
		}

		public void Use(AbilityUseParams abilityUseParams) {
			DealDamage(abilityUseParams);
			PlayParticleEffect();

			playerAudioSource.clip = config.GetAudioClip();
			playerAudioSource.Play();
		}

		private void DealDamage(AbilityUseParams abilityUseParams) {
			float damageToDeal = abilityUseParams.baseDamage + config.GetAbilityBonusDamage();
			abilityUseParams.target.TakeDamage(damageToDeal);
		}

		private void PlayParticleEffect() {
			GameObject particlePrefabToUse = Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity);

			ParticleSystem myParticleSystem = particlePrefabToUse.GetComponent<ParticleSystem>();
			myParticleSystem.Play();
			Destroy(particlePrefabToUse, myParticleSystem.main.duration);
		}
	}
}
