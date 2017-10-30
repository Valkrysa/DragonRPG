using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour {

	public Text textBox;
	private List<string> chatList = new List<string>();

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
