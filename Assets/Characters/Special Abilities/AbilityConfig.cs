using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public abstract class AbilityConfig : ScriptableObject {
		[Header("Special Ability General")]
		[SerializeField] private float energyCost = 10f;
		[SerializeField] private GameObject particlePrefab;
		[SerializeField] private AudioClip[] audioClips;
		[SerializeField] private AnimationClip abilityAnimation;

		protected AbilityBehavior behavior;

		public abstract AbilityBehavior GetBehaviorComponent(GameObject gameObjectToAttachTo);

		public void AttachAbilityTo(GameObject gameObjectToAttachTo) {
			AbilityBehavior behaviorComponent = GetBehaviorComponent(gameObjectToAttachTo);

			behaviorComponent.SetConfig(this);
			behavior = behaviorComponent;
		}

		public void Use(GameObject target) {
			behavior.Use(target);
		}

		public float GetEnergyCost() {
			return energyCost;
		}

		public AnimationClip GetAbilityAnimation() {
			return abilityAnimation;
		}

		public GameObject GetParticlePrefab() {
			return particlePrefab;
		}

		public AudioClip GetRandomAbilitySound() {
			return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
		}
	}
}
