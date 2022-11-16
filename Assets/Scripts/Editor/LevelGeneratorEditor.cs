#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor {

	private LevelGenerator Generator => (LevelGenerator) target;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("Full generation")) {
			Generator.FullGeneration();
			Generator.SpawnPlayer();
		}

		if(GUILayout.Button("Clean all")) {
			Generator.Clean();
		}

		if(GUILayout.Button("Add a room")) {
			Generator.AddRoom();
		}

		if(GUILayout.Button("Force update librairy")) {
			Generator.LoadLibrairy(true);
		}

		if(GUILayout.Button("Toogle Gizmos")) {
			Generator.displayGizmos = !Generator.displayGizmos;
		}
		if(GUILayout.Button("print acceptable")) {
			Generator.PrintAcceptable();
		}

	}

}
#endif