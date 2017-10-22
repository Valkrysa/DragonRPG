using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour {

	public Text textBox;
	private List<string> chatList = new List<string>();

	// Use this for initialization
	void Start () {
		AddChatEntry("You: I'm almost back to the village.");
		Invoke("ChatDelayTest", 2);
	}

	private void ChatDelayTest () {
		AddChatEntry("You: What is that fort doing up ahead?");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddChatEntry (string newEntry) {
		chatList.Insert(0, newEntry);
		UpdateChatBox();
	}

	private void UpdateChatBox () {
		string chatContents = "";

		foreach (string chatEntry in chatList) {
			chatContents += chatEntry + "\n";
		}

		textBox.text = chatContents;
	}
}
