using System.Collections;
using UnityEngine;

public class InGameUI : MonoBehaviour {

	// singelton ??
	// => pas de donotdestroyonload => il faut nullifier l'Instance lors du OnDestroy !
	[SerializeField] private WindowsEntry[] windows = { };

	private void Update() {
		// Toggle visibilities of some windows
		foreach(var we in windows) {
			if(Input.GetButtonDown(we.key)) {
				if(we.windows.gameObject.activeSelf) {
					we.windows.OnHide();
					we.windows.gameObject.SetActive(false);
				} else {
					we.windows.gameObject.SetActive(true);
					we.windows.OnShow();
				}
			}
		}
	}

	[System.Serializable]
	public struct WindowsEntry {
		public string key;
		public WindowsUI windows;
	}
}