using System.Collections;
using UnityEngine;

public class PlayerEntity : ManaOwnerEntity {

	[Header("Player config")]
	[SerializeField] private bool _isMale = true;
	[SerializeField] private Transform spellSource;

	public TMPro.TMP_Text DEBUG_text;

	[Header("Player spells")]
	[SerializeField] private SpellData spell_1;
	[SerializeField] private SpellData spell_2;
	[SerializeField] private SpellData spell_3;

	// Data structures
	private readonly SpellsSet spells = new();
	private readonly StatisticsSet stats = new();
	private PlayerEquipment equipment;

	// References
	private TarodevController.PlayerController _controller;

	// External hooks
	public bool IsFlipX => SpriteRenderer.flipX;
	public bool IsMale => _isMale;
	public Character Character { get; private set; }

	public void LinkCharacter(Character character) {
		if(Character == null) {
			Character = character;
			DontDestroyOnLoad(gameObject);
		} else {
			Debug.LogError("The Character reference in PlayerEntity have already been set.");
		}
	}

	private void Start() {
		_controller = GetComponent<TarodevController.PlayerController>();
		// equipment
		equipment = new(this);
		equipment.onEquipmentChange += () => {
			stats.RecalculateStuff(equipment.GetStuffData());
		};
		// init spells
		UpdateSpellDatas();
	}

	public void UpdateSpellDatas() {
		spells.UpdateSpell(0, spell_1);
		spells.UpdateSpell(1, spell_2);
		spells.UpdateSpell(2, spell_3);
	}

	private void Update() {
		// update cooldown of spells
		spells.UpdateTime();

		if(Input.GetKeyDown(KeyCode.D)) {
			DEBUG_text.text = "Stats=\n" +
			"- health =" + stats[StatisticType.HealthMax] + "\n" +
			"- mana =" + stats[StatisticType.ManaMax] + "\n" +
			"- power =" + stats[StatisticType.ManaPower] + "\n" +
			"- optim =" + stats[StatisticType.ManaOptimisation] + "\n"
			;
		}
	}

	public override EntityType EntityType => EntityType.Player;

	public void RespawnAndDamage(float damages) {
		Damage(damages);
		transform.position = _controller.LastGroundedPosition;
	}

	public bool CastSpell(int index) {
		var spell = spells.GetSpell(index);
		Debug.Log("test " + index + ", => " + spell);
		if(spell.CanCast(this)) {
			spell.Cast(this);
			return true;
		}
		return false;
	}

	public Vector3 GetSpellSource() {
		if(IsFlipX)
			return new Vector3(transform.position.x - spellSource.localPosition.x, spellSource.position.y, spellSource.position.z);
		return spellSource.position;
	}

}