using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : SingletonBehaviour<GlobalGameManager> {
		
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

	private void Start() {
		IsPlaying = true;

		Debug.Log("Game launched with state "+_startState+".");
		CurrentState = _startState;
		
		save = SaveManager.Instance.LoadFromFile();

		FamilyManager.Instance.SetData(save.family);
	}

	public void StartRun(Character character) {
		if(CurrentState != State.FamilyManagement) {
			Debug.LogError("Cannot start a run if we are not doing family management. Current state = " + CurrentState);
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
		if(CurrentState != State.MainMenu && CurrentState != State.RunOver) {
			Debug.LogError("Cannot go to family a run if we are not at main menu OR after a run. Current state = " + CurrentState);
			return;
		}
		CurrentState = State.FamilyManagement;

		// I HOPE I DO NOT FAIT UNE BETISE xd
		var p = GetPlayer();
		if(p)
			Destroy(p.gameObject);

		SceneManager.LoadScene(_sceneFamily_name);
	}
	public void GoToMainMenu() {
		if(CurrentState == State.MainMenu) {
			Debug.LogError("Cannot go to main menu from main menu...  Current state = " + CurrentState);
			return;
		}
		CurrentState = State.MainMenu;

		// I HOPE I DO NOT FAIT UNE BETISE xd
		var p = GetPlayer();
		if(p)
			Destroy(p.gameObject);
		//TODO: stop run ??

		SceneManager.LoadScene(_sceneMainMenu_name);
	}

	private bool quitting = false;
	public void QuitApplication() {
		if(quitting)
			return;
		quitting = true;
		//TODO: operation on quit : SAVING !!


		Debug.Log("Quitting application...");
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit(0);
#endif
	}

	private void OnApplicationQuit() {
		QuitApplication();
	}

	public PlayerEntity GetPlayer() {
		//player should be in DoNotDestroyOnLoad objects
		return FindObjectOfType<PlayerEntity>();
	}

	#region IEnumerators methods
	private IEnumerator _DestroyAndLoad() {
		var loading = SceneManager.LoadSceneAsync(_sceneLoading_name);
		while(!loading.isDone) {
			yield return null;
		}
	}

	private IEnumerator _StartRun() {
		// Destroy everything and put the main scene as loading screen
		yield return _DestroyAndLoad();

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
		sceneData.generator.FullGeneration();

		// Unload the previous Scene
		asyncLoad = SceneManager.UnloadSceneAsync(_sceneLoading_name);
		while(!asyncLoad.isDone) {
			yield return null;
		}

		sceneData.generator.SpawnPlayer(GetPlayer());

		// start the game.
		runStartedTime = UnityEngine.Time.time;
		GetPlayer().LinkCharacter(runCharacterRef);
	}
	#endregion

	#region Pause methods
	public void Pause() {
		IsPlaying = false;
		UnityEngine.Time.timeScale = 0f;
	}

	public void Unpause() {
		IsPlaying = true;
		UnityEngine.Time.timeScale = 1f;
	}
	#endregion

}