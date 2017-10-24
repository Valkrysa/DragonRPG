using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI {
	[RequireComponent(typeof(CameraRaycaster))]
	public class CursorAffordance : MonoBehaviour {

		[SerializeField] const int walkableLayerNumber = 8; // TODO find how to expose to the editor
		[SerializeField] const int enemyLayerNumber = 9; // despite being set as const, without removing serializefield
		[SerializeField] Texture2D walkCursor = null;
		[SerializeField] Texture2D attackCursor = null;
		[SerializeField] Texture2D errorCursor = null;
		[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
		private CameraRaycaster cameraRaycaster;

		// Use this for initialization
		void Start() {
			cameraRaycaster = GetComponent<CameraRaycaster>();
			cameraRaycaster.NotifyLayerChangeObservers += OnCursorLayerChanged;
		}

		void OnCursorLayerChanged(int newLayer) {
			switch (newLayer) {
				case walkableLayerNumber:
					Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
					break;
				case enemyLayerNumber:
					Cursor.SetCursor(attackCursor, cursorHotspot, CursorMode.Auto);
					break;
				default:
					Cursor.SetCursor(errorCursor, cursorHotspot, CursorMode.Auto);
					break;
			}

		}
	}
}
