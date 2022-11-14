using UnityEngine;

public class GlobalGameManager : MonoBehaviour {
		
	public static GlobalGameManager Instance { get; private set; }

	private GlobalSaveState save;

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		Debug.Log("Game started.");
		save = SaveManager.Instance.LoadFromFile();

		FamilyManager.Instance.SetData(save.family);
	}

}