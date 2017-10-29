using System.Collections;
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

		public void Use(AbilityUseParams abilityUseParams) {
			float damageToDeal = abilityUseParams.baseDamage + config.GetAbilityBonusDamage();
			abilityUseParams.target.TakeDamage(damageToDeal);
		}

		public void SetConfig(PowerAttackConfig configToSet) {
			config = configToSet;
		}
	}
}
