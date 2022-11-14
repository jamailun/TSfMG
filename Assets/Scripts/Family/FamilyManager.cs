using UnityEngine;
using System.Collections.Generic;

public class FamilyManager : MonoBehaviour {
		
	public static FamilyManager Instance { get; private set; }

	[SerializeField] private FamilyManagerUI ui_handler;
	private LanguageNames allowedNames;
	private List<CharacterState> characters;

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void SetData(FamilyState state) {
		// load names
		allowedNames = NamesLoader.Load(Language.FR);

		// get characters
		if(state.generation == 0) {
			Debug.Log("generation == 0. Tutorial detected.");
			characters = new();
			characters.Add(CharacterState.Generate(1, allowedNames, true));
		} else {
			characters = new(state.characters);
		}

		// Display data
		ui_handler.Display(state);
	}

}