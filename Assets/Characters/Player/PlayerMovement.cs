using System;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using RPG.CameraUI;

namespace RPG.Characters {
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(AICharacterControl))]
	[RequireComponent(typeof(ThirdPersonCharacter))]
	public class PlayerMovement : MonoBehaviour {

		private Transform myCamera;                  // A reference to the main camera in the scenes transform
		private Vector3 cameraForward;             // The current forward direction of the camera
		private Vector3 movement;

		private ThirdPersonCharacter thirdPersonCharacter = null;
		private CameraRaycaster cameraRaycaster = null;
		private AICharacterControl aICharacterControl = null;
		public GameObject walkTarget;

		private void Start() {
			if (Camera.main != null) {
				myCamera = Camera.main.transform;
			}
			cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
			thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
			aICharacterControl = GetComponent<AICharacterControl>();

			cameraRaycaster.onMouseOverPossiblyWalkable += OnMouseOverPossiblyWalkable;
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
		}

		private void OnMouseOverPossiblyWalkable(Vector3 destination) {
			if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1)) {
				walkTarget.transform.position = destination;
				aICharacterControl.SetTarget(walkTarget.transform);
			}
		}

		private void OnMouseOverEnemy(Enemy enemy) {
			if (Input.GetMouseButton(0)) {
				aICharacterControl.SetTarget(enemy.transform);
			}
		}

		// TODO make this get called again for game pad support
		private void ProcessDirectMovement() {
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			cameraForward = Vector3.Scale(myCamera.forward, new Vector3(1, 0, 1)).normalized;
			movement = v * cameraForward + h * myCamera.right;

			thirdPersonCharacter.Move(movement, false, false);
		}
	}
}
