using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehavior : AbilityBehavior {

		public override void Use(AbilityUseParams abilityUseParams) {
			DealDamage(abilityUseParams);
			PlayParticleEffect();
			PlayAbilitySound();
		}

		private void DealDamage(AbilityUseParams abilityUseParams) {
			float damageToDeal = abilityUseParams.baseDamage + (config as PowerAttackConfig).GetAbilityBonusDamage();
			abilityUseParams.target.TakeDamage(damageToDeal);
		}
	}
}
