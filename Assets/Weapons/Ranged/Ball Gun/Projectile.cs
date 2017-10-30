using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Projectiles {
	public class Projectile : MonoBehaviour {

		[SerializeField] private float projectileSpeed = 1f;
		[SerializeField] private GameObject shooter = null;
		private float damage;
		private const float DESTROY_DELAY = 0.01f;

		// Use this for initialization
		void Start() {

		}

		// Update is called once per frame
		void Update() {

		}

		private void OnCollisionEnter(Collision collision) {
			if (shooter && collision.gameObject.layer != shooter.layer) {
				DamageDamageable(collision);
			}
		}

		private void DamageDamageable(Collision collision) {
			Component damageable = collision.gameObject.GetComponent(typeof(IDamageable));
			if (damageable) {
				(damageable as IDamageable).AdjustHealth(damage);
			}
			Destroy(gameObject, DESTROY_DELAY);
		}

		public void SetDamage(float newDamage) {
			damage = newDamage;
		}

		public void SetShooter(GameObject newShooter) {
			shooter = newShooter;
		}

		public float GetDefaultLaunchSpeed() {
			return projectileSpeed;
		}
	}
}
