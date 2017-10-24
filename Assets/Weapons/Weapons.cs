using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapon {
	[CreateAssetMenu(menuName = "RPG/Weapon")]
	public class Weapons : ScriptableObject {

		public Transform grip;

		[SerializeField] private GameObject weaponPrefab;
		[SerializeField] private AnimationClip attackAnimation;

		public GameObject GetWeaponPrefab() {
			return weaponPrefab;
		}
	}
}
