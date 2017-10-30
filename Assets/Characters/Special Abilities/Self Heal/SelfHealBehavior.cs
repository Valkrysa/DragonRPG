using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class SelfHealBehavior : MonoBehaviour, ISpecialAbility {

		private SelfHealConfig config;
		private Player player;

		// Use this for initialization
		void Start() {
			player = GetComponent<Player>();
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
		}

		private void Heal(AbilityUseParams abilityUseParams) {
			float amountToHeal = config.GetExtraHealth();
			player.AdjustHealth(amountToHeal * -1);

		}

		private void PlayParticleEffect() {
			GameObject particlePrefabToUse = Instantiate(config.GetParticlePrefab(), transform);
			ParticleSystem myParticleSystem = particlePrefabToUse.GetComponent<ParticleSystem>();
			myParticleSystem.Play();
			Destroy(particlePrefabToUse, myParticleSystem.main.duration);
		}
	}
}
