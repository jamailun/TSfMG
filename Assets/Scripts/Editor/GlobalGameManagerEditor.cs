#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalGameManager))]
public class GlobalGameManagerEditor : Editor {

	private GlobalGameManager GGM => (GlobalGameManager) target;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("test player")) {
			var p = GGM.GetPlayer();
			Debug.Log("TEST = "+(p == null ? "NULL":p));
		}


	}

}
#endif