using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	private GameObject playerToFollow;

	// Use this for initialization
	void Start () {
		playerToFollow = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate () {
		transform.position = playerToFollow.transform.position;
	}
}
