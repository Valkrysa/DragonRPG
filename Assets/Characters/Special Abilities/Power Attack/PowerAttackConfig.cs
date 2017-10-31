using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {

	[CreateAssetMenu(menuName = "RPG/Special Ability/Power Attack")]
	public class PowerAttackConfig : AbilityConfig {

		[Header("Power Attack Specific")]
		[SerializeField] private float abilityBonusDamage = 10f;

		public override void AttachComponentTo(GameObject gameObjectToAttachTo) {
			var behaviorComponent = gameObjectToAttachTo.AddComponent<PowerAttackBehavior>();
			behaviorComponent.SetConfig(this);

			behavior = behaviorComponent;
		}

		public float GetAbilityBonusDamage() {
			return abilityBonusDamage;
		}
	}
}

