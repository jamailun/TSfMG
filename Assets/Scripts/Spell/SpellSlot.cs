using UnityEngine;

public class SpellSlot {

	private SpellData spellData;

	private int charges = 0;
	private float nextCharge;
	private float nextAllowed;

	public int ChargesAmount => charges;

	public void SetSpellData(SpellData spellData) {
		if(spellData != this.spellData) {
			this.spellData = spellData;
			charges = 1;
			nextCharge += Time.time + spellData.CooldownCharge;
			nextAllowed += Time.time + spellData.Cooldown;
		}
	}

	public void UpdateCharges() {
		if(spellData != null && charges < spellData.MaxCharges) {
			if(Time.time >= nextCharge) {
				charges++;
				nextCharge += Time.time + spellData.CooldownCharge;
			}
		}
	}

	public bool Cast(PlayerEntity player) {
		if(!CanCast(player)) {
			Debug.Log("cannot cast "+(spellData==null?"NULL":spellData.Name)+" :(");
			return false;
		}
		nextAllowed = Time.time + spellData.Cooldown;
		if(charges == spellData.MaxCharges)
			nextCharge = Time.time + spellData.CooldownCharge;
		charges--;
		player.UseMana(spellData.ManaCost);

		Debug.Log("CAST SPELL " + spellData.Name);
		spellData.Cast(player);

		return true;
	}

	public bool CanCast(PlayerEntity player) {
		return spellData != null &&
				// player is alive
				!player.IsDead &&
				// have charge
				charges > 0 &&
				// player has mana
				player.Mana >= spellData.ManaCost &&
				// not in cooldown
				Time.time >= nextAllowed;
	}

}