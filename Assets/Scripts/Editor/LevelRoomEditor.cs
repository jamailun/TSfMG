#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelRoom))]
public class LevelRoomEditor : Editor {

	private LevelRoom Room => (LevelRoom) target;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("print is prefab")) {
			Debug.Log("Is room " + Room + " prefab ? [" + Room.IsPrefab + "].");
		}


	}

}
#endif