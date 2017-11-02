using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {

	[CreateAssetMenu(menuName = "RPG/Special Ability/Area Effect")]
	public class AreaEffectConfig : AbilityConfig {

		[Header("Area Effect Specific")]
		[SerializeField] private float effectRadius = 5f;
		[SerializeField] private float damageToEachTarget = 15f;

		public override AbilityBehavior GetBehaviorComponent(GameObject gameObjectToAttachTo) {
			return gameObjectToAttachTo.AddComponent<AreaEffectBehavior>();
		}

		public float GetEffectRadius() {
			return effectRadius;
		}

		public float GetDamageToEachTarget() {
			return damageToEachTarget;
		}
	}
}
