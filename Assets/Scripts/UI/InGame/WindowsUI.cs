using UnityEngine;

public abstract class WindowsUI : MonoBehaviour {

	public abstract void OnShow();

	public abstract void OnHide();

	protected void CloseNow() {
		OnHide();
		gameObject.SetActive(false);
	}

}