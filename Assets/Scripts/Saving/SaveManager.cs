using UnityEngine;
using System.IO;
using System;

public class SaveManager : MonoBehaviour {

	public static SaveManager Instance { get; private set; }
	public string FilePath => Path.Combine(Application.persistentDataPath, _fileName);
	public GlobalSaveState SavedData => _saveState;

	[SerializeField] private string _fileName = "save.json";
	[SerializeField] private GlobalSaveState _saveState;

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public static GlobalSaveState CreateDefaultSaveState() {
		long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		return new GlobalSaveState() {
			timeCreated = now,
			timeLastOpened = now,
			id = 1,
			generationNumber = 0, // to start the tutorial I guess
			generationName = "nop"
		};
	}

	public void Save() {
		try {
			string json = JsonUtility.ToJson(_saveState);
			File.WriteAllText(FilePath, json, System.Text.Encoding.UTF8);
			Debug.Log("Save " + json + " into " + FilePath);
		} catch(System.Exception e) {
			Debug.LogError("Error occured (" + e.Message + "). Could not save data.");
		}
	}

	public void LoadFromFile() {
		if(!File.Exists(FilePath)) {
			Debug.Log("No save data has been found at [" + FilePath + "]. Generating data.");
			_saveState = CreateDefaultSaveState();
			return;
		}
		try {
			string json = File.ReadAllText(FilePath, System.Text.Encoding.UTF8);
			Debug.Log("Loaded " + json + " from " + FilePath);
			_saveState = JsonUtility.FromJson<GlobalSaveState>(json);
		} catch(Exception e) {
			Debug.LogError("Error occured (" + e.Message + "). Could not load data. Creating default data.");
			_saveState = CreateDefaultSaveState();
		}
	}
	
}