using UnityEngine;
using TMPro;

public class StatLineUI : MonoBehaviour {

	[SerializeField] private TMP_Text label;

	public void Init(StatisticEntry entry) {
		bool good = entry.Value >= 0;

		string prefix = good ? "+ " : "- ";
		string value = entry.IsMultiplicative ? (entry.Value * 100f).ToString("#") + "%" : entry.Value + "";
		string unit = entry.Type + "";

		label.text = prefix + value + " " + unit;
		label.color = good ? Color.blue : Color.red;
	}

}