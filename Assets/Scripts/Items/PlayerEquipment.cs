using System.Collections.Generic;

public class PlayerEquipment {

	public delegate void OnEquipmentChange();

	private readonly PlayerEntity _player;
	private readonly Dictionary<EquipmentSlot, EquipmentType> _stuff = new();
	private bool IsMale => _player.IsMale;

	public OnEquipmentChange onEquipmentChange;

	public PlayerEquipment(PlayerEntity player) {
		this._player = player;
	}

	public ISet<StatisticEntry> GetStuffData() {
		HashSet<StatisticEntry> set = new();
		foreach(var slot in _stuff.Keys) {
			if(_stuff[slot] != null) {
				set.AddRange(_stuff[slot].Statistics);
			}
		}
		return set;
	}

	public bool CanAccessSlot(EquipmentSlot slot) {
		return !(slot == EquipmentSlot.Cufflink && !IsMale) && !(slot == EquipmentSlot.BowTie && IsMale);
	}

	public EquipmentType GetEquipment(EquipmentSlot slot) {
		if(!CanAccessSlot(slot))
			throw new System.Exception("Cannot access slot "+slot+" with a "+(IsMale ? "male":"female")+" character.");
		return _stuff[slot];
	}
	public void SetEquipment(EquipmentSlot slot, EquipmentType type) {
		if(!CanAccessSlot(slot))
			throw new System.Exception("Cannot access slot " + slot + " with a " + (IsMale ? "male" : "female") + " character.");
		_stuff[slot] = type;
		onEquipmentChange?.Invoke();
	}

	public EquipmentType this[EquipmentSlot slot] {
		get => GetEquipment(slot);
		set => SetEquipment(slot, value);
	}

}