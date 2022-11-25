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
	private StatisticsSet stats;
	private PlayerEquipment equipment;

	// References
	private TarodevController.PlayerController _controller;

	// External hooks
	public bool IsFlipX => SpriteRenderer.flipX;
	public bool IsMale => _isMale;
	public Character Character { get; private set; }

	private void Start() {
		_controller = GetComponent<TarodevController.PlayerController>();
		// stats
		stats = new(this);
		// equipment
		equipment = new(this);
		equipment.onEquipmentChange += () => {
			stats.RecalculateStuff(equipment.GetStuffData());
		};
		// init spells
		UpdateSpellDatas();
		// Link to UI
		_manaBar = InGameUI.Instance.ManaBar;
		_healthBar = InGameUI.Instance.HealthBar;
		_manaBar.Init(0, MaxMana, Mana);
		_healthBar.Init(0, MaxHealth, Health);
	}

	#region External hooks methods
	public override EntityType EntityType => EntityType.Player;

	public void LinkCharacter(Character character) {
		if(Character == null) {
			Character = character;
			Character.onLevelUpEvent += OnLevelUp;
			DontDestroyOnLoad(gameObject);
		} else {
			Debug.LogError("The Character reference in PlayerEntity have already been set.");
		}
	}
	public void UpdateSpellDatas() {
		spells.UpdateSpell(0, spell_1);
		spells.UpdateSpell(1, spell_2);
		spells.UpdateSpell(2, spell_3);
	}

	#endregion

	protected override void EntityUpdate() {
		base.EntityUpdate();

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

	public void RespawnAndDamage(float damages) {
		Damage(damages);
		transform.position = _controller.LastGroundedPosition;
	}

	public bool CastSpell(int index) {
		var spell = spells.GetSpell(index);
		if(spell.CanCast(this)) {
			spell.Cast(this);
			return true;
		}
		return false;
	}

	private void OnLevelUp(int newLevel) {
		Debug.Log("yeay new level (" + newLevel + ")");
		stats.RecalculatePlayerStats();
	}

	private void OnDestroy() {
		// unregister character events
		if(Character != null)
			Character.onLevelUpEvent -= OnLevelUp;
	}

	public Vector3 GetSpellSource() {
		if(IsFlipX)
			return new Vector3(transform.position.x - spellSource.localPosition.x, spellSource.position.y, spellSource.position.z);
		return spellSource.position;
	}

}