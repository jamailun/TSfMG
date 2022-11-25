using UnityEngine;

public class InGameUI : MonoBehaviour {

	[Header("Windows configuration.")]
	[SerializeField] private WindowsEntry[] windows = { };

	[Header("UI configuration")]
	[SerializeField] private BarUI _uiBarHealth;
	[SerializeField] private BarUI _uiBarMana;

	#region External hooks
	public static InGameUI Instance { get; private set; }

	public BarUI HealthBar => _uiBarHealth;
	public BarUI ManaBar => _uiBarMana;

	#endregion

	private void Awake() {
		// Ici on n'utilise PAS de 'donotdestroyonload'. On se gère que nous même.
		if(Instance) {
			Debug.LogWarning("They already is a InGameUI instance ('" + Instance.name + "') from '" + name + "'.");
			gameObject.SetActive(false);
			return;
		}
		Instance = this;
	}

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