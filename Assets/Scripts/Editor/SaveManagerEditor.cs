#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor {

	private SaveManager Manager => (SaveManager) target;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("SAVE")) {
			Manager.Save();
			Debug.Log("Saved.");
		}

		if(GUILayout.Button("LOAD")) {
			Manager.LoadFromFile();
			Debug.Log("Loaded.");
		}


	}

}
#endif