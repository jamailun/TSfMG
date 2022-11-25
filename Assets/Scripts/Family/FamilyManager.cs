using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class FamilyManager : SingletonBehaviour<FamilyManager>, IStateSerializable<FamilyState> {
	
	public bool HaveFamily => FamilyGeneration > 0;

	[SerializeField] private string _fileNamesPath = "Lang/names";
	[SerializeField] private int newFamilyCharactersAmount = 5;
	[Header("Special")]
	[SerializeField] private bool _DEBUG = false;
	private LanguageNames allowedNames;

	// Familty data
	public string FamilyName { get; private set; }
	public int FamilyGeneration { get; private set; }
	private List<Character> _characters;
	public List<Character> FamilyCharacters => new(_characters);
	
	private FamilyManagerUI _uiDisplay;

	private TextAsset NamesAsset => Resources.Load<TextAsset>(_fileNamesPath);

	// Called by the FAmilyManagerUI on start and on destroy. Allows to dynamically handle the display.
	public void SetDisplay(FamilyManagerUI uiDisplay) {
		this._uiDisplay = uiDisplay;
		if(uiDisplay)
			uiDisplay.RefreshDisplay();
	}

	public void CreateNewFamily(string familyName) {
		FamilyName = familyName;
		FamilyGeneration++;
		// clear characters ?? maybe create souls or something ?
		_characters.Clear();

		_characters.Add(new Character(CharacterState.GenerateWithSex(allowedNames, true)));
		_characters.Add(new Character(CharacterState.GenerateWithSex(allowedNames, false)));
		for(int i = 0; i < newFamilyCharactersAmount - 2; i++) {
			_characters.Add(new Character(CharacterState.Generate(allowedNames)));
		}
	}

	public void SetData(FamilyState state) {
		// load names
		allowedNames = NamesLoader.Load(NamesAsset, Language.FR);

		// Read data
		_characters = new List<CharacterState>(state.characters).Select(c => new Character(c)).ToList();
		FamilyGeneration = state.generation;
		FamilyName = state.name;

		// Debug mode
		if(state.generation == 0 && _DEBUG) {
			Debug.Log("generation == 0, but DEBUG MODE => new champ");
			_characters = new();
			_characters.Add(new Character(CharacterState.Generate(allowedNames)));
			FamilyGeneration = 1;
		}

		// Display data
		if(_uiDisplay)
			_uiDisplay.RefreshDisplay();
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

	public void UI_SelectedCharacter(Character character) {
		Debug.Log("The run will start with character " + character);
		GlobalGameManager.Instance.StartRun(character);
	}

	public FamilyState Serialize() {
		return new() {
			name = FamilyName,
			generation = FamilyGeneration,
			characters = _characters.FindAll(c => !c.IsDead).Select(c => c.Serialize()).ToArray()
			//TODO ajouter des trucs pour la suite
		};
	}

}