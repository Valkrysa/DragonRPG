using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;
using RPG.Characters;

public class SpecialAbilities : MonoBehaviour {
	[SerializeField] private AbilityConfig[] _abilitiesConfig;
	[SerializeField] private AudioClip outOfEnergy;
	[SerializeField] private Image energyBar;
	[SerializeField] private float maxEnergyPoints = 100f;
	[SerializeField] private float regenPointsPerTick = 10f;
	[SerializeField] private float regenTickRateInSeconds = 1f;

	private AudioSource audioSource;
	private float currentEnergyPoints = 0f;
	
	void Start () {
		audioSource = GetComponent<AudioSource>();

		currentEnergyPoints = maxEnergyPoints;

		AttachInitialAbilities();

		InvokeRepeating("TickRegen", 0.1f, regenTickRateInSeconds);
	}

	public void ExpendEnergy(float pointsOfEnergyToUse) {
		float newEnergyUnbounded = currentEnergyPoints - pointsOfEnergyToUse;
		currentEnergyPoints = Mathf.Clamp(newEnergyUnbounded, 0f, maxEnergyPoints);

		UpdateEnergyBar();
	}

	public int GetNumberOfAbilities() {
		return _abilitiesConfig.Length;
	}

	public void AttemptSpecialAbility(int abilityIndex, GameObject target = null) {
		float energyCost = _abilitiesConfig[abilityIndex].GetEnergyCost();

		if (energyCost <= currentEnergyPoints) {
			ExpendEnergy(energyCost);
			
			_abilitiesConfig[abilityIndex].Use(target);
		} else {
			audioSource.PlayOneShot(outOfEnergy);
		}
	}

	private void UpdateEnergyBar() {
		energyBar.fillAmount = EnergyAsPercentage();
	}

	private float EnergyAsPercentage() {
		return (float)currentEnergyPoints / (float)maxEnergyPoints;
	}

	private void TickRegen() {
		float newEnergyUnbounded = currentEnergyPoints + regenPointsPerTick;
		currentEnergyPoints = Mathf.Clamp(newEnergyUnbounded, 0f, maxEnergyPoints);
		UpdateEnergyBar();
	}

	private void AttachInitialAbilities() {
		for (int abilityIndex = 0; abilityIndex < _abilitiesConfig.Length; abilityIndex++) {
			_abilitiesConfig[abilityIndex].AttachAbilityTo(gameObject);
		}
	}
}
