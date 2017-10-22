using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {

	[SerializeField] const int walkableLayerNumber = 8; // TODO find how to expose to the editor
	[SerializeField] const int enemyLayerNumber = 9; // despite being set as const, without removing serializefield
	[SerializeField] private float maxHealthPoints = 100;
	[SerializeField] private float damagePerHit = 25;
	[SerializeField] private float minTimeBetweenHits = 0.5f;
	[SerializeField] private float maxAttackRange = 2f;

	private float currentHealthPoints;
	private GameObject currentTarget = null;
	private CameraRaycaster cameraRaycaster;
	private float lastHitTime = 0f;
	
	public float healthAsPercentage {
		get {
			return currentHealthPoints / (float)maxHealthPoints;
		}
	}

	private void Start() {
		currentHealthPoints = maxHealthPoints;

		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
		cameraRaycaster.notifyMouseClickObservers += OnMouseClicked;
	}

	private void OnMouseClicked(RaycastHit raycastHit, int layerHit) {
		if (layerHit == 9) {
			GameObject enemy = raycastHit.collider.gameObject;

			float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
			if (distanceToEnemy > maxAttackRange) {
				return; // quit early if the enemy is out of range
			}

			currentTarget = enemy;

			Component damageable = enemy.GetComponent(typeof(IDamageable));
			if ( damageable && (Time.time - lastHitTime > minTimeBetweenHits) ) {
				(damageable as IDamageable).TakeDamage(damagePerHit);
				lastHitTime = Time.time;
			}
		}
	}

	public void TakeDamage (float damage) {
		currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
	}
}
