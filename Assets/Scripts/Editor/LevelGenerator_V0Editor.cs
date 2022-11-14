#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator_V0))]
public class LevelGenerator_V0Editor : Editor {

	private LevelGenerator_V0 Generator => (LevelGenerator_V0) target;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("COMPLETE GENERATION")) {
			Generator.Generate();
			Generator.SeparateRooms();
			Generator.Filter();
			Generator.Triangulate();
			Generator.AddSecondaryRooms();
			Generator.TriangulateSecondary();
			SceneView.RepaintAll();
		}/*
		if(GUILayout.Button("GENERATE ROOMS")) {
			Generator.Generate();
			SceneView.RepaintAll();
		}
		if(GUILayout.Button("SOLVE ROOMS")) {
			Generator.SeparateRooms();
			SceneView.RepaintAll();
		}
		if(GUILayout.Button("FILTER ROOMS")) {
			Generator.Filter();
			SceneView.RepaintAll();
		}
		if(GUILayout.Button("TRIANGULATE ROOMS")) {
			Generator.Triangulate();
			SceneView.RepaintAll();
		}
		if(GUILayout.Button("ADD SECONDARY ROOMS")) {
			Generator.AddSecondaryRooms();
			SceneView.RepaintAll();
		}
		if(GUILayout.Button("2nd TRIANGULATION")) {
			Generator.TriangulateSecondary();
			SceneView.RepaintAll();
		}*/
		if(GUILayout.Button("Toggle gray display")) {
			Generator.DisplayGray = !Generator.DisplayGray;
			SceneView.RepaintAll();
		}

	}

}
#endif