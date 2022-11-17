using UnityEngine;
using UnityEngine.UI;

public class BasicMainMenuUI : MonoBehaviour {

	[SerializeField] private Button familyButton;

	private void Start() {
		var buttonText = familyButton.GetComponentInChildren<TMPro.TMP_Text>();
		if(FamilyManager.Instance.HaveFamily) {
			buttonText.text = "Continue the game";
		} else {
			buttonText.text = "Create a new family";
		}
	}

}