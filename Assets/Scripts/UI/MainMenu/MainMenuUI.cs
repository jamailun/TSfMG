using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour {

	[SerializeField] private BasicMainMenuUI mainPage;
	[SerializeField] private FamilyCreateNameUI familyPage;

	private void Start() {
		DisplayMain();
	}

	private void DisplayMain() {
		mainPage.gameObject.SetActive(true);
		familyPage.gameObject.SetActive(false);
	}

	public void UI_PressedFamilyStart() {
		
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

}