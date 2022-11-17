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

	private int MaxPage => FamilyManager.Instance.FamilyCharacters.Count / charactersPerPage;
	private int characterPage = 0;

	private void Start() {
		FamilyManager.Instance.SetDisplay(this);
	}

	// Called by the FamilyManager instance
	public void RefreshDisplay() {
		familyName_label.text = FamilyManager.Instance.FamilyName;

		RefreshCharactersDisplays();
	}

	private void OnDestroy() {
		// remove ref from the manager
		FamilyManager.Instance.SetDisplay(null);
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
		var chars = FamilyManager.Instance.FamilyCharacters.Slice(charactersPerPage * characterPage, charactersPerPage);
		foreach(var c in chars) {
			var display = Instantiate(characterUiPrefab, charactersLayout.transform);
			display.Init(c);
		}
		leftPageButton.interactable = characterPage > 0;
		rightPageButton.interactable = characterPage < MaxPage;
	}

	// Called by children
	public void SelectedCharacter(Character character) {
		foreach(var btn in GetComponentsInChildren<Button>())
			btn.interactable = false;
		FamilyManager.Instance.UI_SelectedCharacter(character);
	}

}