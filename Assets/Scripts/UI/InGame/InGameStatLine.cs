using UnityEngine;
using TMPro;

public class InGameStatLine : MonoBehaviour {

	[SerializeField] private TMP_Text label_type;
	[SerializeField] private TMP_Text label_value;

	public void Set(StatisticType type, float value) {
		label_type.text = type + " :";
		label_value.text = value.ToString("#");
	}

}