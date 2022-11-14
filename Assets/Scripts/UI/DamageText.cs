using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour {

	[SerializeField] private float speed = .8f;
	[SerializeField] private float unscalePerSecond = 2f;

	[Header("Colors")]
	[SerializeField] private Color colorPlayerDamage;
	[SerializeField] private Color colorPlayerHeal;
	[SerializeField] private Color colorEnemyDamage;
	[SerializeField] private Color colorEnemyHeal;
	[SerializeField] private Color colorBlocked;

	private string displayedValue;
	private DamageType damageType;

	private Vector3 source, velocity;

	private Color GetColor(DamageType type) {
		return type switch {
			DamageType.PlayerDamage => colorPlayerDamage,
			DamageType.PlayerHeal => colorPlayerHeal,
			DamageType.EnemyDamage => colorEnemyDamage,
			DamageType.EnemyHeal => colorEnemyHeal,
			DamageType.Blocked => colorBlocked,
			_ => colorPlayerDamage
		};
	}

	public static DamageType GetTypeFromEntityType(EntityType type, bool damage) {
		return type switch {
			EntityType.Player => damage ? DamageType.PlayerDamage : DamageType.PlayerHeal,
			_ => damage ? DamageType.EnemyDamage : DamageType.EnemyHeal,
		};
	}

	public enum DamageType {
		PlayerDamage,
		PlayerHeal,
		EnemyDamage,
		EnemyHeal,
		Blocked
	}

	public void SetDamageText(string value, Vector3 source, DamageType type) {
		this.displayedValue = value;
		this.source = source;
		damageType = type;

		velocity = new Vector3(Random.Range(-1, 1), Random.Range(.5f, 2), 0) * speed;
	}

	private void Start() {
		transform.position = source - new Vector3(0, 0, 0.3f);

		var text = gameObject.GetComponent<TextMeshPro>();

		text.text = displayedValue;
		text.color = GetColor(damageType);

		Destroy(gameObject, .5f);
	}


	private void Update() {
		transform.position += (velocity * Time.deltaTime);
		transform.localScale -= Time.deltaTime * unscalePerSecond * Vector3.one;
	}

}