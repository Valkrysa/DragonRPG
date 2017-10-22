using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float projectileSpeed = 1f;
	private float damage;

	// Use this for initialization
	void Start () {
		Invoke("SelfDestruct", 5f); // IF I DON'T COLLIDE IN 5 SECONDS I SELF DESTRUCT
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void SelfDestruct () {
		Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision collision) {
		Component damageable = collision.gameObject.GetComponent(typeof(IDamageable));
		if (damageable) {
			(damageable as IDamageable).TakeDamage(damage);
			Destroy(gameObject);
		}
	}

	public void SetDamage (float newDamage) {
		damage = newDamage;
	}
}
