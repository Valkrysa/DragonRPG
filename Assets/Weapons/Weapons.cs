﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapon {
	[CreateAssetMenu(menuName = "RPG/Weapon")]
	public class Weapons : ScriptableObject {

		public Transform grip;

		[SerializeField] private GameObject weaponPrefab;
		[SerializeField] private AnimationClip attackAnimation;
		[SerializeField] private float minTimeBetweenHits = 0.5f;
		[SerializeField] private float maxAttackRange = 2f;
		[SerializeField] private float additionalDamage = 10f;

		public GameObject GetWeaponPrefab() {
			return weaponPrefab;
		}

		public AnimationClip GetAttackAnimClip() {
			RemoveAnimationEvents(); // the asset pack we're using has a bunch of events we don't want
			return attackAnimation;
		}

		private void RemoveAnimationEvents() {
			attackAnimation.events = new AnimationEvent[0];
		}

		public float GetMinTimeBetweenHits() {
			return minTimeBetweenHits;
		}

		public float GetMaxAttackRange() {
			return maxAttackRange;
		}

		public float GetAdditionalDamage() {
			return additionalDamage;
		}
	}
}
