using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;
using UnityEditor.Animations;

namespace RPG.Characters {
	[SelectionBase]
	public class Character : MonoBehaviour {
		[Header("Animator")]
		[SerializeField]
		private AnimatorController animatorController;
		[SerializeField] private AnimatorOverrideController animatorOverrideController;
		[SerializeField] private Avatar avatar;

		[Header("Audio Source")]
		[SerializeField] private float audioSourceSpatialBlend = 0.5f;

		[Header("Capsule Collider")]
		[SerializeField] private Vector3 colliderCenter;
		[SerializeField] private float colliderRadius;
		[SerializeField] private float colliderHeight;

		[Header("Movement")]
		[SerializeField] private float moveSpeedMultiplier = 1.5f;
		[SerializeField] private float animationSpeedMultiplier = 1.5f;
		[SerializeField] private float moveThreshold = 1f;
		[SerializeField] private float movingTurnSpeed = 360;
		[SerializeField] private float stationaryTurnSpeed = 180;

		[Header("NavMeshAgent")]
		[SerializeField] private float navMeshAgentSteeringSpeed = 1.0f;
		[SerializeField] private float navMeshAgentStoppingDistance = 1.3f;

		private NavMeshAgent navMeshAgent;
		private Vector3 movement;
		private Animator myAnimator;
		private Rigidbody myRigidbody;
		private float turnAmount;
		private float forwardAmount;

		private void Awake() {
			AddRequiredComponents();
		}

		private void Start() {
			CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
			cameraRaycaster.onMouseOverPossiblyWalkable += OnMouseOverPossiblyWalkable;
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
		}

		private void Update() {
			if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) {
				Move(navMeshAgent.desiredVelocity);
			} else {
				Move(Vector3.zero);
			}
		}

		private void OnAnimatorMove() {
			if (Time.deltaTime > 0) {
				Vector3 velocity = (myAnimator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

				velocity.y = myRigidbody.velocity.y;
				myRigidbody.velocity = velocity;
			}
		}

		public void Kill() {
			// TODO allow death signaling
		}

		private void OnMouseOverPossiblyWalkable(Vector3 destination) {
			if (Input.GetMouseButton(0)) {
				navMeshAgent.SetDestination(destination);
			}
		}

		private void OnMouseOverEnemy(Enemy enemy) {
			if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(1)) {
				navMeshAgent.SetDestination(enemy.transform.position);
			}
		}

		private void AddRequiredComponents() {
			myAnimator = gameObject.AddComponent<Animator>();
			myAnimator.runtimeAnimatorController = animatorController;
			myAnimator.avatar = avatar;

			var collider = gameObject.AddComponent<CapsuleCollider>();
			collider.center = colliderCenter;
			collider.radius = colliderRadius;
			collider.height = colliderHeight;

			myRigidbody = gameObject.AddComponent<Rigidbody>();

			navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
			navMeshAgent.updatePosition = true;
			navMeshAgent.updateRotation = false;
			navMeshAgent.autoBraking = false;
			navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;

			var audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.spatialBlend = audioSourceSpatialBlend;
		}

		public void Move(Vector3 movement) {
			SetForwardAndTurn(movement);
			ApplyExtraTurnRotation();
			UpdateAnimator();
		}

		private void SetForwardAndTurn(Vector3 movement) {
			if (movement.magnitude > moveThreshold) {
				movement.Normalize();
			}
			var localMovement = transform.InverseTransformDirection(movement);
			turnAmount = Mathf.Atan2(localMovement.x, localMovement.z);
			forwardAmount = localMovement.z;
		}

		void UpdateAnimator() {
			myAnimator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			myAnimator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			myAnimator.speed = animationSpeedMultiplier;
		}

		void ApplyExtraTurnRotation() {
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}
	}
}
