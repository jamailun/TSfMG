using System.Collections.Generic;
using UnityEngine;

public class PlayerStuff : MonoBehaviour {

	private readonly bool isMale;
	private readonly Dictionary<EquipmentSlot, EquipmentType> stuff = new();

	public PlayerStuff(bool isMale) {
		this.isMale = isMale;
	}

	public ISet<StatisticEntry> GetStuffData() {
		HashSet<StatisticEntry> set = new();
		foreach(var slot in stuff.Keys) {
			if(stuff[slot] != null) {
				set.AddRange(stuff[slot].Statistics);
			}
		}
		return set;
	}

	public bool CanAccessSlot(EquipmentSlot slot) {
		return !(slot == EquipmentSlot.Cufflink && !isMale) && !(slot == EquipmentSlot.BowTie && isMale);
	}

	public EquipmentType GetEquipment(EquipmentSlot slot) {
		if(!CanAccessSlot(slot))
			throw new System.Exception("Cannot access slot "+slot+" with a "+(isMale?"male":"female")+" character.");
		return stuff[slot];
	}
	public void SetEquipment(EquipmentSlot slot, EquipmentType type) {
		if(!CanAccessSlot(slot))
			throw new System.Exception("Cannot access slot " + slot + " with a " + (isMale ? "male" : "female") + " character.");
		stuff[slot] = type;
	}

	public EquipmentType this[EquipmentSlot slot] {
		get => GetEquipment(slot);
		set => SetEquipment(slot, value);
	}

}