using UnityEngine;
using System.Collections.Generic;

public class EquipmentType : ItemType {

	[SerializeField] private EquipmentSlot _slot;
	public EquipmentSlot Slot => _slot;

	[SerializeField] private StatisticEntry[] _statistics;
	public HashSet<StatisticEntry> Statistics => new(_statistics);


}