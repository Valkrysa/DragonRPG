using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatReaction : MonoBehaviour {

	private ChatBox chatBox = null;
	private bool reacted = false;

	public string reaction = "NPC: Hello there!";

	// Use this for initialization
	void Start () {
		chatBox = FindObjectOfType<ChatBox>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void React () {
		if (!reacted) {
			reacted = true;
			chatBox.AddChatEntry(reaction);
		}
	}
}
