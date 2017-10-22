using System;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (NavMeshAgent))]
[RequireComponent(typeof(AICharacterControl))]
[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour {
	
	[SerializeField] const int walkableLayerNumber = 8; // TODO find how to expose to the editor
	[SerializeField] const int enemyLayerNumber = 9; // despite being set as const, without removing serializefield
	
	private Transform myCamera;                  // A reference to the main camera in the scenes transform
	private Vector3 cameraForward;             // The current forward direction of the camera
	private Vector3 movement;
	
	private ThirdPersonCharacter thirdPersonCharacter = null;
	private CameraRaycaster cameraRaycaster = null;
	private AICharacterControl aICharacterControl = null;
	public GameObject walkTarget;

	private bool isInDirectMode = false;

	private void Start() {
		if (Camera.main != null) {
			myCamera = Camera.main.transform;
		}
		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
		aICharacterControl = GetComponent<AICharacterControl>();

		cameraRaycaster.notifyMouseClickObservers += OnMouseClicked;
	}

	private void OnMouseClicked (RaycastHit raycastHit, int layerHit) {
		switch (layerHit) {
			case walkableLayerNumber:
				walkTarget.transform.position = raycastHit.point;
				aICharacterControl.target = walkTarget.transform;
				break;
			case enemyLayerNumber:
				// aICharacterControl.target = raycastHit.transform;
				GameObject enemy = raycastHit.collider.gameObject;
				aICharacterControl.target = enemy.transform;
				break;
			default:
				return;
		}
	}

	// TODO make this get called again for game pad support
	private void ProcessDirectMovement() {
		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");

		cameraForward = Vector3.Scale(myCamera.forward, new Vector3(1, 0, 1)).normalized;
		movement = v * cameraForward + h * myCamera.right;

		thirdPersonCharacter.Move(movement, false, false);
	}
}

