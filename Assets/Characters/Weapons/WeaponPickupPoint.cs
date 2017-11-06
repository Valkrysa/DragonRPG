using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.Characters {
	[ExecuteInEditMode]
	public class WeaponPickupPoint : MonoBehaviour {
		[SerializeField] private WeaponConfig weaponConfigConfig;
		[SerializeField] private AudioClip pickupSoundEffect;
		
		void Update() {
			if (!Application.isPlaying) {
				InstantiateWeapon();
				DestroyChildren();
			}
		}

		private void InstantiateWeapon() {
			var weapon = weaponConfigConfig.GetWeaponPrefab();
			weapon.transform.position = Vector3.zero;
			Instantiate(weapon, gameObject.transform);
		}

		private void DestroyChildren() {
			foreach (Transform child in transform) {
				DestroyImmediate(child.gameObject);
			}
		}

		private void OnTriggerEnter(Collider otherCollider) {
			//GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponSystem>().PutWeaponInHand(weaponConfigConfig);
			FindObjectOfType<PlayerControl>().GetComponent<WeaponSystem>().PutWeaponInHand(weaponConfigConfig);
			GetComponent<AudioSource>().PlayOneShot(pickupSoundEffect);
		}
	}
}

