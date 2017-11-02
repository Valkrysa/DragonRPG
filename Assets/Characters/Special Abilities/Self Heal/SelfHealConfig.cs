using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {

	[CreateAssetMenu(menuName = "RPG/Special Ability/Self Heal")]
	public class SelfHealConfig : AbilityConfig {

		[Header("Self Heal Specific")]
		[SerializeField] private float extraHealth = 50f;

		public override AbilityBehavior GetBehaviorComponent(GameObject gameObjectToAttachTo) {
			return gameObjectToAttachTo.AddComponent<SelfHealBehavior>();
		}

		public float GetExtraHealth() {
			return extraHealth;
		}
	}
}
