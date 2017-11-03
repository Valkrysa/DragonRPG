using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehavior : AbilityBehavior {
		public override void Use(GameObject target) {
			DealDamage(target);
			PlayParticleEffect();
			PlayAbilitySound();
		}

		private void DealDamage(GameObject target) {
			float damageToDeal = (config as PowerAttackConfig).GetAbilityBonusDamage();
			target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
		}
	}
}
