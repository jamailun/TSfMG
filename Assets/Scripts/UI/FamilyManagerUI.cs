using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FamilyManagerUI : MonoBehaviour {

	[SerializeField] private TMP_Text familyName_label;

	[Header("Characters display")]
	[SerializeField] private CharacterDisplayUI characterUiPrefab;
	[SerializeField] private LayoutGroup charactersLayout;
	[SerializeField] private int charactersPerPage = 5;
	[SerializeField] private Button leftPageButton;
	[SerializeField] private Button rightPageButton;

	private FamilyManager _familyManager;
	private int MaxPage => _familyManager.FamilyCharacters.Count / charactersPerPage;
	private int characterPage = 0;

	// Called by the FamilyManager instance
	public void InitializeDisplay(FamilyManager familyManager) {
		this._familyManager = familyManager;
		familyName_label.text = familyManager.FamilyName + ", page "+characterPage+"/"+MaxPage;

		RefreshCharactersDisplays();
	}

	// Called by nextPageButton
	public void NextCharacterPage() {
		if(characterPage >= MaxPage)
			return;
		characterPage++;
		RefreshCharactersDisplays();
	}

	// Called by previousPageButton
	public void PreviousCharacterPage() {
		if(characterPage <= 0)
			return;
		characterPage--;
		RefreshCharactersDisplays();
	}

	private void RefreshCharactersDisplays() {
		charactersLayout.transform.DestroyChildren();
		var chars = _familyManager.FamilyCharacters.Slice(charactersPerPage * characterPage, charactersPerPage);
		Debug.Log("refresh (page=" + characterPage + "/" + MaxPage + "). count = " + chars.Count);
		foreach(var c in chars) {
			var display = Instantiate(characterUiPrefab, charactersLayout.transform);
			display.Init(c);
		}
		leftPageButton.interactable = characterPage > 0;
		rightPageButton.interactable = characterPage < MaxPage;
	}

	// Called by children
	public void SelectedCharacter(Character character) {
		Debug.Log("TODO start run with " + character);
	}

}