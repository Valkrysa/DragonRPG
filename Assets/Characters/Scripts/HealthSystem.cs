using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters {
	public class HealthSystem : MonoBehaviour {
		[SerializeField] private AudioClip[] damageSounds;
		[SerializeField] private AudioClip[] deathSounds;
		[SerializeField] private Image healthBar;
		[SerializeField] private float maxHealthPoints = 100f;
		[SerializeField] private float deathVanishSeconds = 2f;

		private Animator animator;
		private AudioSource audioSource;
		private Character _character;
		private float currentHealthPoints;
		private const string DEATH_TRIGGER = "Death";
		
		void Start() {
			animator = GetComponent<Animator>();
			audioSource = GetComponent<AudioSource>();
			_character = GetComponent<Character>();

			SetCurrentMaxHealth();
		}
		
		void Update() {
			UpdateHealthBar();
		}

		public float HealthAsPercentage() {
			return currentHealthPoints / (float) maxHealthPoints;
		}

		private void UpdateHealthBar() {
			if (healthBar) {
				healthBar.fillAmount = HealthAsPercentage();
			}
		}

		public void TakeDamage(float damage) {
			bool characterDies = (currentHealthPoints - damage <= 0);

			currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
			PlayRandomDamageSound();

			if (characterDies) {
				StartCoroutine(KillCharacter());
			}
		}

		public void Heal(float healingAmount) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints + healingAmount, 0, maxHealthPoints);
		}

		private void SetCurrentMaxHealth() {
			currentHealthPoints = maxHealthPoints;
		}

		private void PlayRandomDamageSound() {
			int randomIndex = UnityEngine.Random.Range(0, damageSounds.Length);
			AudioClip clip = damageSounds[randomIndex];
			audioSource.PlayOneShot(clip);
		}

		private float PlayRandomDeathSound() {
			int randomIndex = UnityEngine.Random.Range(0, deathSounds.Length);
			AudioClip clip = deathSounds[randomIndex];
			audioSource.PlayOneShot(clip);

			return deathSounds[randomIndex].length;
		}

		private IEnumerator KillCharacter() {
			StopAllCoroutines();
			_character.Kill();
			animator.SetTrigger(DEATH_TRIGGER);

			// if we are dealing with a _playerControl
			PlayerControl playerControlComponent = GetComponent<PlayerControl>();
			if (playerControlComponent && playerControlComponent.isActiveAndEnabled) {
				float deathLength = PlayRandomDeathSound();
				yield return new WaitForSecondsRealtime(deathLength);

				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			} else { // assume enemy
				DestroyObject(gameObject, deathVanishSeconds);
			}
		}
	}
}

