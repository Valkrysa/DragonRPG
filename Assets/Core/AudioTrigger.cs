using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.Core {
	public class AudioTrigger : MonoBehaviour {

		[SerializeField] AudioClip clip;
		[SerializeField] float distanceToPlayerTriggerRadius = 5f;
		[SerializeField] bool isOneTimeOnly = true;

		private ChatReaction chatReaction;
		private AudioSource audioSource;
		private GameObject player;
		private bool hasPlayed = false;

		void Start() {
			chatReaction = GetComponent<ChatReaction>();
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.clip = clip;

			player = FindObjectOfType<PlayerControl>().gameObject;
		}

		private void Update() {
			float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
			if (distanceToPlayer <= distanceToPlayerTriggerRadius) {
				RequestPlayAudioClip();
			}
		}

		void RequestPlayAudioClip() {
			if (isOneTimeOnly && hasPlayed) {
				return;
			} else if (audioSource.isPlaying == false) {
				audioSource.Play();
				hasPlayed = true;

				if (chatReaction) {
					chatReaction.React();
				}
			}
		}

		void OnDrawGizmos() {
			Gizmos.color = new Color(0, 255f, 0, .5f);
			Gizmos.DrawWireSphere(transform.position, distanceToPlayerTriggerRadius);
		}
	}
}
