using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {

	public struct AbilityUseParams {
		public IDamageable target;
		public float baseDamage;

		public AbilityUseParams(IDamageable newTarget, float newBaseDamage) {
			target = newTarget;
			baseDamage = newBaseDamage;
		}
	}

	public abstract class SpecialAbility : ScriptableObject {
		[Header("Special Ability General")]
		[SerializeField] private float energyCost = 10f;

		protected ISpecialAbility behavior;
		
		public abstract void AttachComponentTo(GameObject gameObjectToAttachTo);

		public void Use(AbilityUseParams abilityUseParams) {
			behavior.Use(abilityUseParams);
		}

		public float GetEnergyCost() {
			return energyCost;
		}
	}

	public interface ISpecialAbility {
		void Use(AbilityUseParams abilityUseParams);
	}
}
