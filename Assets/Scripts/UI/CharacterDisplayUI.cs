using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDisplayUI : MonoBehaviour {

	private FamilyManagerUI _parent;
	[SerializeField] private TMP_Text label_name;
	[SerializeField] private Image display;

	private Character currentCharacter = null;

	private void Awake() {
		_parent = GetComponentInParent<FamilyManagerUI>();
		if(!_parent)
			Debug.LogError("Could NOT find parent for characterDisplayUI '" + gameObject.name + "'.");
	}

	public void Init(Character character) {
		this.currentCharacter = character;
		label_name.text = character.Name;
		//TODO display
	}

	public void SelectedCharacter() {
		if(currentCharacter == null)
			return;
		_parent.SelectedCharacter(currentCharacter);
	}

}