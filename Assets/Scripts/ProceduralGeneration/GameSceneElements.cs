using UnityEngine.SceneManagement;

public struct GameSceneElements {

	public LevelGenerator generator;

	public GameSceneElements(Scene scene) {
		generator = null;

		foreach(var obj in scene.GetRootGameObjects()) {
			if(generator == null && obj.GetComponent<LevelGenerator>() != null) {
				generator = obj.GetComponent<LevelGenerator>();
			}
		}
	}

	public bool IsValid { get { return generator != null; } }

}