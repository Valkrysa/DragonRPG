using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Characters {
	public class AreaEffectBehavior : AbilityBehavior {
		public override void Use(GameObject target) {
			DealRadialDamage(target);
			PlayParticleEffect();
			PlayAbilitySound();
			PlayAbilityAnimation();
		}

		private void DealRadialDamage(GameObject target) {
			float damageToDeal = (config as AreaEffectConfig).GetDamageToEachTarget();

			RaycastHit[] targetsHit = Physics.SphereCastAll(
				transform.position,
				(config as AreaEffectConfig).GetEffectRadius(),
				Vector3.up,
				(config as AreaEffectConfig).GetEffectRadius()
			);
			foreach (var targetHit in targetsHit) {
				HealthSystem hitDamageable = targetHit.collider.gameObject.GetComponent<HealthSystem>();
				bool hitPlayer = targetHit.collider.gameObject.GetComponent<PlayerControl>();
				if (!hitPlayer && hitDamageable != null) {
					hitDamageable.TakeDamage(damageToDeal);
				}
			}
		}
	}
}
