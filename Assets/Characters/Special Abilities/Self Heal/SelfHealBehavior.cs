using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class SelfHealBehavior : AbilityBehavior {
		public override void Use(GameObject target) {
			Heal(target);
			PlayParticleEffect();
			PlayAbilitySound();
			PlayAbilityAnimation();
		}

		private void Heal(GameObject target) {
			HealthSystem health = GetComponent<HealthSystem>();
			health.Heal((config as SelfHealConfig).GetExtraHealth());
		}
	}
}
