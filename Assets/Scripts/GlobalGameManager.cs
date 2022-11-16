using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour {
		
	public static GlobalGameManager Instance { get; private set; }
	private GlobalSaveState save;

	public State CurrentState { get; private set; }
	// time of the GAME, not the application
	public float Time => save.timePlayed + UnityEngine.Time.time;

	[Header("Start only")]
	[SerializeField] private State _startState;

	[Header("Scenes configuration")]
	[SerializeField] private string _sceneMainMenu = "Scene_MainMenu";
	[SerializeField] private string _sceneGame = "Scene_Game";
	[SerializeField] private string _sceneFamily = "Scene_Family";
	[SerializeField] private string _sceneLoading = "Scene_Loading";

	// current run
	private float runStartedTime;
	private Character runCharacterRef;
	//

	public enum State {
		MainMenu,
		FamilyManagement,
		InRun
	}


	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		Debug.Log("Game launched with state "+_startState+".");
		CurrentState = _startState;
		
		save = SaveManager.Instance.LoadFromFile();

		FamilyManager.Instance.SetData(save.family);
	}

	public void StartRun(Character character) {
		if(CurrentState != State.FamilyManagement) {
			Debug.LogError("Cannot start a run if we are not doing familty management. Current state = " + CurrentState);
			return;
		}
		if(character == null) {
			Debug.LogError("Cannot start run with a null or dead character.");
			return;
		}
		CurrentState = State.InRun;
		runCharacterRef = character;
		StartCoroutine(_StartRun());
	}

	private IEnumerator _DestroyAndLoad() {
		var loading = SceneManager.LoadSceneAsync(_sceneLoading);
		while(!loading.isDone) {
			yield return null;
		}
	}

	private IEnumerator _StartRun(/*PlayerEntity player*/) {
		// Destroy everything and put the main scene as loading screen
		yield return _DestroyAndLoad();

		PlayerEntity player = null;

		// The Application loads the Scene in the background at the same time as the current Scene.
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneGame, LoadSceneMode.Additive);
		while(!asyncLoad.isDone) {
			yield return null;
		}
		var target = SceneManager.GetSceneByName(_sceneGame);

		//TODO do operations in loaded scene
		// générer le donjon


		// Unload the previous Scene
		asyncLoad = SceneManager.UnloadSceneAsync(_sceneLoading);
		while(!asyncLoad.isDone) {
			yield return null;
		}

		// start the game.
		runStartedTime = UnityEngine.Time.time;
		player.LinkCharacter(runCharacterRef);
	}

}