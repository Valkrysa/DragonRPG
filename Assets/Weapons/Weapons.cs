using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Weapon")]
public class Weapons : ScriptableObject
{

	public Transform grip;

	[SerializeField] private GameObject weaponPrefab;
	[SerializeField] private AnimationClip attackAnimation;
	
	public GameObject GetWeaponPrefab() {
		return weaponPrefab;
	}
}
