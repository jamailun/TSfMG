using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class FamilyManager : MonoBehaviour, IStateSerializable<FamilyState> {
		
	public static FamilyManager Instance { get; private set; }

	[SerializeField] private FamilyManagerUI ui_handler;
	[SerializeField] private string _fileNamesPath = "Lang/names";
	private LanguageNames allowedNames;

	// Familty data
	private string _familyName;
	private int _familyGeneration;
	private List<Character> _characters;

	private TextAsset NamesAsset => Resources.Load<TextAsset>(_fileNamesPath);

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
		allowedNames = NamesLoader.Load(NamesAsset, Language.FR);

		// get data
		if(state.generation == 0) {
			_characters = new();
			_characters.Add(new Character(CharacterState.Generate(allowedNames, true, 25f)));
			Debug.Log("generation == 0. Tutorial detected.");
			//TODO: truc pour le tutoriel ??? i guess xd
			_familyGeneration = 1;
		} else {
			_characters = new List<CharacterState>(state.characters).Select(c => new Character(c)).ToList();
			_familyGeneration = state.generation;
		}
		_familyName = state.name;

		// Display data
		ui_handler.Display(state);
	}

	public void RunOver(float yearsElapsed) {
		foreach(var c in _characters) {
			if(c.IsDead)
				continue;

			c.YearsPassed(yearsElapsed);
			if(c.IsDead) {
				Debug.Log("Oh no, " + c.Name + " is DEAD of old age.");
			}
		}
		_characters.RemoveAll(c => c.IsDead);
	}

	public FamilyState Serialize() {
		return new() {
			name = _familyName,
			generation = _familyGeneration,
			characters = _characters.FindAll(c => !c.IsDead).Select(c => c.Serialize()).ToArray()
			//TODO ajouter des trucs pour la suite
		};
	}

}