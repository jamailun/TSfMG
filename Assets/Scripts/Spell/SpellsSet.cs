using System.Collections;
using UnityEngine;

public class SpellsSet {

	public const int SPELL_AMOUNT = 3;

	private readonly SpellSlot[] slots = new SpellSlot[SPELL_AMOUNT];

	public SpellsSet() {
		for(int i = 0; i < SPELL_AMOUNT; i++)
			slots[i] = new();
	}

	public void UpdateSpell(int index, SpellData data) {
		CheckValidity(index);
		slots[index].SetSpellData(data);
	}

	public void UpdateTime() {
		for(int i = 0; i < SPELL_AMOUNT; i++)
			slots[i].UpdateCharges();
	}

	private void CheckValidity(int index) {
		if(index < 0 || index >= SPELL_AMOUNT)
			throw new System.Exception("Could not get invalid-index spell : " + index + ".");
	}

	public SpellSlot GetSpell(int index) {
		CheckValidity(index);
		return slots[index];
	}


}