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

	public abstract class AbilityConfig : ScriptableObject {
		[Header("Special Ability General")]
		[SerializeField] private float energyCost = 10f;
		[SerializeField] private GameObject particlePrefab = null;
		[SerializeField] private AudioClip[] audioClips = null;

		protected AbilityBehavior behavior;

		public abstract AbilityBehavior GetBehaviorComponent(GameObject gameObjectToAttachTo);

		public void AttachAbilityTo(GameObject gameObjectToAttachTo) {
			AbilityBehavior behaviorComponent = GetBehaviorComponent(gameObjectToAttachTo);

			behaviorComponent.SetConfig(this);
			behavior = behaviorComponent;
		}

		public void Use(AbilityUseParams abilityUseParams) {
			behavior.Use(abilityUseParams);
		}

		public float GetEnergyCost() {
			return energyCost;
		}

		public GameObject GetParticlePrefab() {
			return particlePrefab;
		}

		public AudioClip GetRandomAbilitySound() {
			return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
		}
	}
}
