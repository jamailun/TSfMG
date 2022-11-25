using UnityEngine;

public class InGameUI : SingletonBehaviour<InGameUI> {

	[Header("Windows configuration.")]
	[SerializeField] private WindowsEntry[] windows = { };

	[Header("UI configuration")]
	[SerializeField] private BarUI _uiBarHealth;
	[SerializeField] private BarUI _uiBarMana;

	#region External hooks

	public BarUI HealthBar => _uiBarHealth;
	public BarUI ManaBar => _uiBarMana;

	#endregion

	public override bool DontDestroyObjectOnLoad => false;

	// on veut pouvoir remplacer l'instance.
	private void OnDestroy() {
		if(Instance == this)
			Instance = null;
	}

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