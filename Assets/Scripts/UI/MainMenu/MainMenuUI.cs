using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	[SerializeField] private BasicMainMenuUI mainPage;
	[SerializeField] private FamilyCreateNameUI familyPage;

	private void Start() {
		mainPage.gameObject.SetActive(true);
		familyPage.gameObject.SetActive(false);
	}

	public void UI_PressedFamilyStart() {
		if(FamilyManager.Instance.HaveFamily) {
			// continue == on va dans la gestion de famille
			GlobalGameManager.Instance.GoToFamily();
		} else {
			// Il faut commencer par créer une famille
			familyPage.gameObject.SetActive(true);
			mainPage.gameObject.SetActive(false);
			familyPage.Init(this);
		}
	}

	public void UI_PressedHelp() {
		Debug.Log("Pressed help :)");
	}

	public void UI_PressedCredits() {
		Debug.Log("Pressed credits :)");
	}

	public void UI_PressedQuit() {
		GlobalGameManager.Instance.QuitApplication();
	}

	// FAMILY NAME
	public void FamilyNameChosen(string familyName) {
		FamilyManager.Instance.CreateNewFamily(familyName);
		// then we can go to family
		GlobalGameManager.Instance.GoToFamily();
	}

}