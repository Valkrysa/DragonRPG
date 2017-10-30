﻿using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehavior : MonoBehaviour, ISpecialAbility {

		private PowerAttackConfig config;

		// Use this for initialization
		void Start() {

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
		}

		private void DealDamage(AbilityUseParams abilityUseParams) {
			float damageToDeal = abilityUseParams.baseDamage + config.GetAbilityBonusDamage();
			abilityUseParams.target.AdjustHealth(damageToDeal);
		}

		private void PlayParticleEffect() {
			GameObject particlePrefabToUse = Instantiate(config.GetParticlePrefab(), transform);
			ParticleSystem myParticleSystem = particlePrefabToUse.GetComponent<ParticleSystem>();
			myParticleSystem.Play();
			Destroy(particlePrefabToUse, myParticleSystem.main.duration);
		}
	}
}
