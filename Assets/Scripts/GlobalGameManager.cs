using UnityEngine;
using System.Collections;

public class GlobalGameManager : MonoBehaviour {
		
	public static GlobalGameManager Instance { get; private set; }
	private GlobalSaveState save;

	public State CurrentState { get; private set; }
	// time of the GAME, not the application
	public float Time => save.timePlayed + UnityEngine.Time.time;

	[Header("Start only")]
	[SerializeField] private State _startState;

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

	private IEnumerator _StartRun() {
		// faire un fondu au noir
		// changer de scène
		// écran de chargement
		// générer le donjon
		//
		yield return new WaitForSeconds(0.1f);
	}

}