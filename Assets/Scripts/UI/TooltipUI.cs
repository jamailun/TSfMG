using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour {

	private static TooltipUI Instance { get; set; }

	[Header("Tooltip config")]
	[SerializeField] private TMP_Text mainLabel;
	[SerializeField] private RectTransform statsContainer;
	[SerializeField] private TMP_Text descriptionLabel;
	[SerializeField] private Vector2 mouseOffset;

	[Header("Prefab config")]
	[SerializeField] private StatLineUI linePrefab;

	// fields
	private RectTransform parentRectTransform;
	private Vector2 localPoint;

	private void Awake() {
		if(Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		parentRectTransform = transform.parent.GetComponent<RectTransform>();
		HideTooltip();
	}

	private void Update() {
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, Input.mousePosition, Camera.current, out localPoint);
		transform.localPosition = localPoint + mouseOffset;
	}

	private void UpdateContent(ItemType type) {
		// Clean
		statsContainer.DestroyChildren();
		// Display
		mainLabel.text = type.Name;
		descriptionLabel.text = type.Description;

		var eType = type as EquipmentType;
		if(eType != null) {
			foreach(var stat in eType.Statistics) {
				var line = Instantiate(linePrefab, statsContainer);
				line.Init(stat);
			}
		}
	}
	private void UpdateContent(Character character) {
		// Clean
		statsContainer.DestroyChildren();
		// Display
		mainLabel.text = character.Name;
		descriptionLabel.text = "";
		// Curses
		var line = Instantiate(linePrefab, statsContainer);
		line.Init("hop malédictions");
	}

	public static void ShowTooltip(ItemType item) {
		if(Instance == null) {
			Debug.LogWarning("No tooltip set for the scene !");
			return;
		}
		Instance.gameObject.SetActive(true);
		Instance.UpdateContent(item);
		Instance.Update();
	}

	public static void ShowTooltip(Character character) {
		if(Instance == null) {
			Debug.LogWarning("No tooltip set for the scene !");
			return;
		}
		Instance.gameObject.SetActive(true);
		Instance.UpdateContent(character);
		Instance.Update();
	}

	public static void HideTooltip() {
		if(Instance != null)
			Instance.gameObject.SetActive(false);
	}


}