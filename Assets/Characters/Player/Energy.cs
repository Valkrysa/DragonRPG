using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;
using RPG.Characters;

public class Energy : MonoBehaviour {

	[SerializeField] private Image energyBar;
	[SerializeField] private float maxEnergyPoints = 100f;
	[SerializeField] private float regenPointsPerTick = 10f;
	[SerializeField] private float regenTickRateInSeconds = 1f;

	private float currentEnergyPoints = 0f;
	
	void Start () {
		currentEnergyPoints = maxEnergyPoints;

		InvokeRepeating("TickRegen", 0.1f, regenTickRateInSeconds);
	}

	public void ExpendEnergy(float pointsOfEnergyToUse) {
		float newEnergyUnbounded = currentEnergyPoints - pointsOfEnergyToUse;
		currentEnergyPoints = Mathf.Clamp(newEnergyUnbounded, 0f, maxEnergyPoints);

		UpdateEnergyBar();
	}

	public bool IsEnergyAvailable(float pointsOfEnergyToCheck) {
		return pointsOfEnergyToCheck <= currentEnergyPoints;
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
}
