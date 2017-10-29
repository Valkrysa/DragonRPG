using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public class AreaEffectBehavior : MonoBehaviour, ISpecialAbility {

		private AreaEffectConfig config;

		// Use this for initialization
		void Start() {

		}

		// Update is called once per frame
		void Update() {

		}

		public void Use(AbilityUseParams abilityUseParams) {
			float damageToDeal = abilityUseParams.baseDamage + config.GetDamageToEachTarget();

			RaycastHit[] targetsHit = Physics.SphereCastAll(
				transform.position, 
				config.GetEffectRadius(), 
				Vector3.up,
				config.GetEffectRadius()
			);
			foreach (var targetHit in targetsHit) {
				IDamageable hitDamageable = targetHit.collider.gameObject.GetComponent<IDamageable>();
				if (hitDamageable != null) {
					hitDamageable.TakeDamage(damageToDeal);
				}
				
			}
		}

		public void SetConfig(AreaEffectConfig configToSet) {
			config = configToSet;
		}
	}
}
