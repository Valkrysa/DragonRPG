using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {

	[CreateAssetMenu(menuName = "RPG/Special Ability/Area Effect")]
	public class AreaEffectConfig : AbilityConfig {

		[Header("Power Attack Specific")]
		[SerializeField] private float effectRadius = 5f;
		[SerializeField] private float damageToEachTarget = 15f;

		public override void AttachComponentTo(GameObject gameObjectToAttachTo) {
			var behaviorComponent = gameObjectToAttachTo.AddComponent<AreaEffectBehavior>();
			behaviorComponent.SetConfig(this);

			behavior = behaviorComponent;
		}

		public float GetEffectRadius() {
			return effectRadius;
		}

		public float GetDamageToEachTarget() {
			return damageToEachTarget;
		}
	}
}
