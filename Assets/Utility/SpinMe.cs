using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
	public class SpinMe : MonoBehaviour {

		[SerializeField] float xRotationsPerMinute = 1f;
		[SerializeField] float yRotationsPerMinute = 1f;
		[SerializeField] float zRotationsPerMinute = 1f;

		void Update() {
			float degreesInRotation = 360f;
			float secondsPerMinute = 60f;

			float xDegreesPerFrame = ((degreesInRotation * xRotationsPerMinute) / secondsPerMinute) * Time.deltaTime;
			transform.RotateAround(transform.position, transform.right, xDegreesPerFrame);

			float yDegreesPerFrame = ((degreesInRotation * yRotationsPerMinute) / secondsPerMinute) * Time.deltaTime;
			transform.RotateAround(transform.position, transform.up, yDegreesPerFrame);

			float zDegreesPerFrame = ((degreesInRotation * zRotationsPerMinute) / secondsPerMinute) * Time.deltaTime;
			transform.RotateAround(transform.position, transform.forward, zDegreesPerFrame);
		}
	}
}
