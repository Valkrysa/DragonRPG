using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public class AreaEffectBehavior : AbilityBehavior {

		public override void Use(AbilityUseParams abilityUseParams) {
			DealRadialDamage(abilityUseParams);
			PlayParticleEffect();
			PlayAbilitySound();
		}

		private void DealRadialDamage(AbilityUseParams abilityUseParams) {
			float damageToDeal = abilityUseParams.baseDamage + (config as AreaEffectConfig).GetDamageToEachTarget();

			RaycastHit[] targetsHit = Physics.SphereCastAll(
				transform.position,
				(config as AreaEffectConfig).GetEffectRadius(),
				Vector3.up,
				(config as AreaEffectConfig).GetEffectRadius()
			);
			foreach (var targetHit in targetsHit) {
				IDamageable hitDamageable = targetHit.collider.gameObject.GetComponent<IDamageable>();
				bool hitPlayer = targetHit.collider.gameObject.GetComponent<Player>();
				if (!hitPlayer && hitDamageable != null) {
					hitDamageable.TakeDamage(damageToDeal);
				}
			}
		}
	}
}
