using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour {
		
	public static GlobalGameManager Instance { get; private set; }
	private GlobalSaveState save;

	public State CurrentState { get; private set; }
	// time of the GAME, not the application
	public float Time => save.timePlayed + UnityEngine.Time.time;
	public bool IsPlaying { get; private set; }

	[Header("Start only")]
	[SerializeField] private State _startState;

	[Header("Scenes configuration")]
	[SerializeField] private string _sceneMainMenu_name = "Scene_MainMenu";
	[SerializeField] private string _sceneGame_name = "Scene_Game";
	[SerializeField] private string _sceneFamily_name = "Scene_Family";
	[SerializeField] private string _sceneLoading_name = "Scene_Loading";

	// current run
	private float runStartedTime;
	private Character runCharacterRef;
	//

	public enum State {
		MainMenu,
		FamilyManagement,
		InRun,
		RunOver
	}


	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
		IsPlaying = true;
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
		if(character == null || character.IsDead) {
			Debug.LogError("Cannot start run with a null or dead character.");
			return;
		}
		CurrentState = State.InRun;
		runCharacterRef = character;
		StartCoroutine(_StartRun());
	}

	public void GoToFamily() {
		if(CurrentState != State.MainMenu || CurrentState != State.RunOver) {
			Debug.LogError("Cannot start a run if we are not doing familty management. Current state = " + CurrentState);
			return;
		}
	}

	private IEnumerator _DestroyAndLoad() {
		var loading = SceneManager.LoadSceneAsync(_sceneLoading_name);
		while(!loading.isDone) {
			yield return null;
		}
	}

	private IEnumerator _StartRun(/*PlayerEntity player*/) {
		// Destroy everything and put the main scene as loading screen
		yield return _DestroyAndLoad();


		//TODO: JE NE SAIS PAS SI CECI FONCTIONNE !!!!
		PlayerEntity player = FindObjectOfType<PlayerEntity>();
		Debug.Log("find for player." + (player == null ? " NOT FOUND" : "Found : " + player));



		// The Application loads the Scene in the background at the same time as the current Scene.
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneGame_name, LoadSceneMode.Additive);
		while(!asyncLoad.isDone) {
			yield return null;
		}
		var scene = SceneManager.GetSceneByName(_sceneGame_name);
		var sceneData = new GameSceneElements(scene);
		if(!sceneData.IsValid) {
			Debug.LogError("INVALID scene data for scene " + _sceneGame_name + ".");
		}

		// Generate the donjon
		sceneData.generator.Clean(false);
		sceneData.generator.FullGeneration(player);

		//maintenant on laisse le loading
		Debug.Log("on va laisser un peu l'écran de laoding :)");
		yield return new WaitForSeconds(3f);


		// Unload the previous Scene
		asyncLoad = SceneManager.UnloadSceneAsync(_sceneLoading_name);
		while(!asyncLoad.isDone) {
			yield return null;
		}

		// start the game.
		runStartedTime = UnityEngine.Time.time;
		player.LinkCharacter(runCharacterRef);
	}

	public void Pause() {
		IsPlaying = false;
		UnityEngine.Time.timeScale = 0f;
	}

	public void Unpause() {
		IsPlaying = true;
		UnityEngine.Time.timeScale = 1f;
	}

}