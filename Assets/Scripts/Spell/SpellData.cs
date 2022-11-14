using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "SpellData", menuName = "proto/SpellData", order = 1)]
public class SpellData : ScriptableObject {

	[Header("Général")]
	[SerializeField] private string _name;
	[SerializeField] private string _description;

	[Header("Mana and cooldowns")]
	[SerializeField] private float _manaCost;
	[SerializeField] private float _cooldown; // time between casts
	[SerializeField] private float _cooldownCharge; // time to get another charge
	[SerializeField] private int _maxCharges = 1;

	[Header("Effect")]
	[SerializeField] private float castingTime;
	//todo casting animation
	[SerializeField] private SpellProduction producedPrefab;
	[SerializeField] private float postTime;

	#region External hook

	public string Description => _description;
	public string Name => _name;

	public float ManaCost => _manaCost;
	public float Cooldown => _cooldown;
	public float CooldownCharge => _cooldownCharge;
	public int MaxCharges => _maxCharges;

	#endregion

	public IEnumerable Cast(PlayerEntity player) { // plus facile de rajouter des args ici

		// real casting time
		yield return new WaitForSeconds(castingTime);

		// Spawn production
		if(producedPrefab) {
			var spell = Instantiate(producedPrefab);
			if(player.IsFlipX) {
				spell.SetFlipX();
				spell.transform.position = player.GetSpellSource() - spell.LocalPosition;
			} else {
				spell.transform.position = player.GetSpellSource() - spell.LocalPosition;
			}
		}

		// period after the spell
		yield return new WaitForSeconds(postTime);
	}

}