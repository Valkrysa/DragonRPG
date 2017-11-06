using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointContainer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnDrawGizmos() {
		Vector3 firstPos = transform.GetChild(0).position;
		Vector3 previousPos = firstPos;

		foreach (Transform waypoint in transform) {
			Gizmos.DrawSphere(waypoint.position, 0.3f);
			Gizmos.DrawLine(previousPos, waypoint.position);
			previousPos = waypoint.position;
		}
		Gizmos.DrawLine(previousPos, firstPos);
		
		/*
		my original implementation, teachers was more concise
		Vector3 previousPos = Vector3.zero;
		Vector3 currentPos = Vector3.zero;
		Vector3 firstPos = Vector3.zero;
		int iteration = 0;

		foreach (Transform waypoint in transform) {
			iteration++;
			Gizmos.DrawSphere(waypoint.position, 0.3f);

			if (currentPos == Vector3.zero) {
				currentPos = waypoint.position;
				firstPos = currentPos;
			} else {
				previousPos = currentPos;
				currentPos = waypoint.position;

				Gizmos.DrawLine(previousPos, currentPos);
			}

			if (iteration == transform.childCount) {
				Gizmos.DrawLine(currentPos, firstPos);
			}
		}
		*/
	}
}
