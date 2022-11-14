using System.Collections;
using UnityEngine;

public class PlayerEntity : ManaOwnerEntity {

	[Header("Player config")]
	[SerializeField] private Transform spellSource;

	[Header("Player spells")]
	[SerializeField] private SpellData spell_1;
	[SerializeField] private SpellData spell_2;
	[SerializeField] private SpellData spell_3;

	private readonly SpellsSet spells = new();
	private TarodevController.PlayerController _controller;
	public bool IsFlipX => SpriteRenderer.flipX;

	private void Start() {
		_controller = GetComponent<TarodevController.PlayerController>();
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