#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleMonster))]
public class SimpleMonsterEditor : Editor {

	private SimpleMonster Monster => (SimpleMonster) target;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("flip x ("+ Monster.GoingRight+ ")")) {
			Monster._debug_Flipx();
		}


	}

}
#endif