using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class FamilyCreateNameUI : MonoBehaviour {

	[SerializeField] private TMP_InputField nameInput;
	[SerializeField] private TMP_Text errorMessage;
	[Header("Name configuration")]
	[SerializeField] private int minSize = 3;
	[SerializeField] private string regex = "^[A-Za-z\\-\\s]+$";

	private MainMenuUI _parent;

	public void Init(MainMenuUI parent) {
		nameInput.text = "";
		this._parent = parent;
	}

	public void UI_PressedValidate() {
		errorMessage.text = "";

		string name = nameInput.text;
		if(name.Replace(" ", "").Length < minSize) {
			errorMessage.text = "Name too short !";
			return;
		}

		Match match = Regex.Match(name, regex, RegexOptions.Singleline);
		if(name == string.Empty || ! match.Success) {
			errorMessage.text = "This name is not acceptable.";
			return;
		}

		_parent.FamilyNameChosen(name);
	}

}