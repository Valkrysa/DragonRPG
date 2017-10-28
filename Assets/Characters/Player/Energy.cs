using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;
using RPG.Characters;

public class Energy : MonoBehaviour {

	[SerializeField] const int walkableLayerNumber = 8; // TODO find how to expose to the editor
	[SerializeField] const int enemyLayerNumber = 9; // despite being set as const, without removing serializefield
	[SerializeField] private RawImage energyBar;
	[SerializeField] private float maxEnergyPoints = 100f;
	[SerializeField] private float pointsPerHit = 10f;

	private float currentEnergyPoints = 0f;
	private CameraRaycaster cameraRaycaster = null;

	// Use this for initialization
	void Start () {
		currentEnergyPoints = maxEnergyPoints;
		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
		cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
	}
	
	// Update is called once per frame
	void Update () {

	}

	private void OnMouseOverEnemy(Enemy enemy) {
		if (Input.GetMouseButtonDown(1)) {
			ExpendEnergy();
		}
	}

	public void ExpendEnergy() {
		float newEnergyUnbounded = currentEnergyPoints - pointsPerHit;
		currentEnergyPoints = Mathf.Clamp(newEnergyUnbounded, 0f, maxEnergyPoints);

		UpdateEnergyBar();
	}

	private void UpdateEnergyBar() {
		float xValue = -(EnergyAsPercentage() / 2f) - 0.5f;
		energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
	}

	public float GetEnergy() {
		return currentEnergyPoints;
	}

	private float EnergyAsPercentage() {
		return (float)currentEnergyPoints / (float)maxEnergyPoints;
	}
}
