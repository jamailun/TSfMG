using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This UI class is used to represent any value in a bar.
/// </summary>
public class BarUI : MonoBehaviour, Subscribed<float> {

	private const string FORMAT = "#";

	[SerializeField] private Image content;
	[SerializeField] private TMPro.TMP_Text label;
	[SerializeField] private float _min = 0f;
	[SerializeField] private float _max = 100f;
	private float value;

	private void Start() {
		if(content == null) {
			Debug.LogError("BarUI " + gameObject.name + " don't have any 'content' element.");
			enabled = false;
			return;
		}
		value = _max;
		UpdateImage();
	}

	/// <summary>
	/// Init the bar from the code.
	/// </summary>
	/// <param name="minValue">The minimal value of the bar.</param>
	/// <param name="maxValue">The maximal value of the bar.</param>
	/// <param name="value">The current value of the bar.</param>
	public void Init(float minValue, float maxValue, float value) {
		this._min = minValue;
		this._max = maxValue;
		SetValue(value); // this will also update the image.
	}

	public void SetValue(float value) {
		this.value = Mathf.Max(_min, Mathf.Min(_max, value));
		UpdateImage();
	}

	private void UpdateImage() {
		if(label != null) {
			if(value == 0)
				label.text = "0/"+_max.ToString(FORMAT);
			else
				label.text = value.ToString(FORMAT) + "/" + _max.ToString(FORMAT);
		}
		if(_max <= 0) {
			content.fillAmount = 0f;
			return;
		}
		content.fillAmount = (value - _min) / _max;
	}

	public void SubscribeUpdate(float t) {
		SetValue(t);
	}
}
