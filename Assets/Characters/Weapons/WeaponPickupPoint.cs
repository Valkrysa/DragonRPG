using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.Characters {
	[ExecuteInEditMode]
	public class WeaponPickupPoint : MonoBehaviour {
		[SerializeField] private Weapons weaponConfig;
		[SerializeField] private AudioClip pickupSoundEffect;

		private PlayerControl _playerControl = null;
		private AudioSource myAudioSource = null;
		
		void Start() {
			myAudioSource = GetComponent<AudioSource>();
			_playerControl = FindObjectOfType<PlayerControl>();
		}
		
		void Update() {
			if (!Application.isPlaying) {
				InstantiateWeapon();
				DestroyChildren();
			}
		}

		private void InstantiateWeapon() {
			var weapon = weaponConfig.GetWeaponPrefab();
			weapon.transform.position = Vector3.zero;
			Instantiate(weapon, gameObject.transform);
		}

		private void DestroyChildren() {
			foreach (Transform child in transform) {
				DestroyImmediate(child.gameObject);
			}
		}

		private void OnTriggerEnter(Collider otherCollider) {
			_playerControl.PutWeaponInHand(weaponConfig);
			myAudioSource.PlayOneShot(pickupSoundEffect);
		}
	}
}

