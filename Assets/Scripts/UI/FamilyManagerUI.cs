using UnityEngine;
using TMPro;

public class FamilyManagerUI : MonoBehaviour {

	[SerializeField] private TMP_Text familyName_label;

	public void Display(FamilyState state) {
		familyName_label.text = state.name;
		
	}

}