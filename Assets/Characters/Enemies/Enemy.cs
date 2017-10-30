using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Projectiles;
using RPG.Core;

namespace RPG.Characters {
	public class Enemy : MonoBehaviour, IDamageable {

		[SerializeField] GameObject projectileToFire = null;
		[SerializeField] GameObject projectileSocket = null;
		[SerializeField] private float maxHealthPoints = 100;
		[SerializeField] private float attackRadius = 4f;
		[SerializeField] private float chaseRadius = 10f;
		[SerializeField] private float damagePerShot = 9f;
		[SerializeField] private float secondsBetweenShots = 0.5f;
		[SerializeField] private float shotTimeVariation = 0.1f;
		[SerializeField] private Vector3 aimOffset = new Vector3(0f, 1f, 0f);

		private float currentHealthPoints = 100;
		private AICharacterControl aICharacterControl = null;
		private Player player = null;
		private bool isAttacking = false;
		private ChatReaction chatReaction = null;


		public float healthAsPercentage {
			get {
				return currentHealthPoints / (float)maxHealthPoints;
			}
		}

		private void Start() {
			currentHealthPoints = maxHealthPoints;
			
			player = FindObjectOfType<Player>();
			aICharacterControl = GetComponent<AICharacterControl>();
			chatReaction = GetComponent<ChatReaction>();
		}

		private void Update() {
			// stop taking "Enemy" actions if player dies.
			if (player.healthAsPercentage <= Mathf.Epsilon) {
				StopAllCoroutines();
				Destroy(this);
			}

			float distanceToTarget = Vector3.Distance(player.transform.position, transform.position);

			if (distanceToTarget <= attackRadius) {
				aICharacterControl.SetTarget(transform);
			} else if (distanceToTarget <= chaseRadius) {
				aICharacterControl.SetTarget(player.transform);
			} else if (distanceToTarget > chaseRadius) {
				aICharacterControl.SetTarget(transform);
			}

			if (distanceToTarget <= attackRadius && !isAttacking) {
				isAttacking = true;

				float timeBetweenShots = Random.Range(secondsBetweenShots - shotTimeVariation, secondsBetweenShots + shotTimeVariation);

				InvokeRepeating("FireProjectile", 0f, timeBetweenShots);

				if (chatReaction) {
					chatReaction.React();
				}
			}
			if (distanceToTarget > attackRadius) {
				isAttacking = false;
				CancelInvoke();
			}
		}

		private void OnDrawGizmos() {
			// draw attack sphere
			Gizmos.color = new Color(255f, 0f, 0f, .5f);
			Gizmos.DrawWireSphere(transform.position, attackRadius);

			// draw move sphere
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, chaseRadius);
		}

		private void FireProjectile() {
			GameObject projectileFired = Instantiate(projectileToFire, projectileSocket.transform.position, Quaternion.identity);
			Projectile projectileComponent = projectileFired.GetComponent<Projectile>();
			projectileComponent.SetShooter(gameObject);
			projectileComponent.SetDamage(damagePerShot);

			Vector3 targetDirection = (player.transform.position + aimOffset - projectileFired.transform.position).normalized;
			float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
			projectileFired.GetComponent<Rigidbody>().velocity = targetDirection * projectileSpeed;
		}

		public void AdjustHealth(float damage) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);

			if (currentHealthPoints <= 0) {
				Destroy(gameObject);
			}
		}
	}
}
