using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public class AreaEffectBehavior : MonoBehaviour, ISpecialAbility {

		private AreaEffectConfig config;
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

		public void SetConfig(AreaEffectConfig configToSet) {
			config = configToSet;
		}

		public void Use(AbilityUseParams abilityUseParams) {
			DealRadialDamage(abilityUseParams);
			PlayParticleEffect();

			playerAudioSource.clip = config.GetAudioClip();
			playerAudioSource.Play();
		}

		private void DealRadialDamage(AbilityUseParams abilityUseParams) {
			float damageToDeal = abilityUseParams.baseDamage + config.GetDamageToEachTarget();

			RaycastHit[] targetsHit = Physics.SphereCastAll(
				transform.position,
				config.GetEffectRadius(),
				Vector3.up,
				config.GetEffectRadius()
			);
			foreach (var targetHit in targetsHit) {
				IDamageable hitDamageable = targetHit.collider.gameObject.GetComponent<IDamageable>();
				bool hitPlayer = targetHit.collider.gameObject.GetComponent<Player>();
				if (!hitPlayer && hitDamageable != null) {
					hitDamageable.TakeDamage(damageToDeal);
				}
			}
		}

		private void PlayParticleEffect() {
			GameObject particlePrefab = config.GetParticlePrefab();
			GameObject particlePrefabToUse = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);

			ParticleSystem myParticleSystem = particlePrefabToUse.GetComponent<ParticleSystem>();
			myParticleSystem.Play();
			Destroy(particlePrefabToUse, myParticleSystem.main.duration);
		}
	}
}
