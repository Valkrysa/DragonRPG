using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {

	[CreateAssetMenu(menuName = "RPG/Special Ability/Power Attack")]
	public class PowerAttackConfig : AbilityConfig {

		[Header("Power Attack Specific")]
		[SerializeField] private float abilityBonusDamage = 10f;

		public override AbilityBehavior GetBehaviorComponent(GameObject gameObjectToAttachTo) {
			return gameObjectToAttachTo.AddComponent<PowerAttackBehavior>();
		}

		public float GetAbilityBonusDamage() {
			return abilityBonusDamage;
		}
	}
}

