using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Animations;

namespace RPG.Characters {
	[SelectionBase]
	public class Character : MonoBehaviour {
		[Header("Animator")]
		[SerializeField]
		private AnimatorController animatorController;
		[SerializeField] private AnimatorOverrideController animatorOverrideController;
		[SerializeField] private Avatar avatar;
		[SerializeField] [Range(0.1f, 1f)] private float animatorForwardCap = 1f;

		[Header("Audio Source")]
		[SerializeField] private float audioSourceSpatialBlend = 0.5f;

		[Header("Capsule Collider")]
		[SerializeField] private Vector3 colliderCenter = new Vector3(0, 1.03f, 0);
		[SerializeField] private float colliderRadius = 0.2f;
		[SerializeField] private float colliderHeight = 2.03f;

		[Header("Movement")]
		[SerializeField] private float moveSpeedMultiplier = .7f;
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
		private bool isAlive = true;

		private void Awake() {
			AddRequiredComponents();
		}

		private void Update() {
			if (!navMeshAgent.isOnNavMesh) {
				Debug.LogError(gameObject.name + " is not properly placed onto the nav mesh");
			} else if (isAlive && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) {
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
			isAlive = false;
		}

		public void SetDestination(Vector3 worldPos) {
			navMeshAgent.destination = worldPos;
		}

		public AnimatorOverrideController GetAnimatorOverrideController() {
			return animatorOverrideController;
		}

		public float GetAnimationSpeedMultiplier() {
			return myAnimator.speed;
		}

		private void Move(Vector3 movement) {
			SetForwardAndTurn(movement);
			ApplyExtraTurnRotation();
			UpdateAnimator();
		}

		private void AddRequiredComponents() {
			var collider = gameObject.AddComponent<CapsuleCollider>();
			collider.center = colliderCenter;
			collider.radius = colliderRadius;
			collider.height = colliderHeight;

			myRigidbody = gameObject.AddComponent<Rigidbody>();
			myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

			var audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.spatialBlend = audioSourceSpatialBlend;

			myAnimator = gameObject.AddComponent<Animator>();
			myAnimator.runtimeAnimatorController = animatorController;
			myAnimator.avatar = avatar;

			navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
			navMeshAgent.speed = navMeshAgentSteeringSpeed;
			navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
			navMeshAgent.autoBraking = false;
			navMeshAgent.updateRotation = false;
			navMeshAgent.updatePosition = true;
		}

		private void SetForwardAndTurn(Vector3 movement) {
			if (movement.magnitude > moveThreshold) {
				movement.Normalize();
			}
			var localMovement = transform.InverseTransformDirection(movement);
			turnAmount = Mathf.Atan2(localMovement.x, localMovement.z);
			forwardAmount = localMovement.z;
		}

		private void UpdateAnimator() {
			myAnimator.SetFloat("Forward", forwardAmount * animatorForwardCap, 0.1f, Time.deltaTime);
			myAnimator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
			myAnimator.speed = animationSpeedMultiplier;
		}

		private void ApplyExtraTurnRotation() {
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}
	}
}
