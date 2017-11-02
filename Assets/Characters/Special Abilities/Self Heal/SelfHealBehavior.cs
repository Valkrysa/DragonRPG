using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class SelfHealBehavior : AbilityBehavior {

		public override void Use(AbilityUseParams abilityUseParams) {
			Heal(abilityUseParams);
			PlayParticleEffect();
			PlayAbilitySound();
		}

		private void Heal(AbilityUseParams abilityUseParams) {
			Player player = GetComponent<Player>();
			player.Heal((config as SelfHealConfig).GetExtraHealth());

		}
	}
}
