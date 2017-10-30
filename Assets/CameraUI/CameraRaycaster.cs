using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using RPG.Characters;

namespace RPG.CameraUI {
	public class CameraRaycaster : MonoBehaviour {
		[SerializeField] const int clickToWalkLayer = 8;
		[SerializeField] Texture2D walkCursor = null;
		[SerializeField] Texture2D attackCursor = null;
		[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

		private float maxRaycastDepth = 100f; // Hard coded value
		
		public delegate void OnMouseOverTerrain(Vector3 destination);
		public event OnMouseOverTerrain onMouseOverPossiblyWalkable;

		public delegate void OnMouseOverEnemy(Enemy enemy);
		public event OnMouseOverEnemy onMouseOverEnemy;

		void Update() {
			// Check if pointer is over an interactable UI element
			if (EventSystem.current.IsPointerOverGameObject()) {
				// Put UI interaction here
			} else {
				PerformRayCasts();
			}
		}

		private void PerformRayCasts() {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (RaycastForEnemy(ray)) {
				return;
			} else if (RaycastForWalkable(ray)) {
				return;
			}
		}

		private bool RaycastForWalkable(Ray ray) {
			RaycastHit hitInfo;
			LayerMask possiblyWalkableLayer = 1 << clickToWalkLayer;

			bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, possiblyWalkableLayer);
			if (potentiallyWalkableHit) {
				Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
				onMouseOverPossiblyWalkable(hitInfo.point);
				return true;
			}

			return false;
		}

		private bool RaycastForEnemy(Ray ray) {
			RaycastHit hitInfo;
			bool raycastHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth);

			if (raycastHit) {
				GameObject gameObjectHit = hitInfo.collider.gameObject;
				Enemy enemyHit = gameObjectHit.GetComponent<Enemy>();
				if (enemyHit) {
					Cursor.SetCursor(attackCursor, cursorHotspot, CursorMode.Auto);
					onMouseOverEnemy(enemyHit);
					return true;
				}
			}

			return false;
		}
	}
}

