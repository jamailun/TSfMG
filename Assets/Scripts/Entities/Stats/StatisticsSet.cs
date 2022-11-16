using System.Collections.Generic;

public class StatisticsSet {

	private readonly Dictionary<StatisticType, float> _stuff_flat = new();
	private readonly Dictionary<StatisticType, float> _stuff_mult = new();

	private readonly Dictionary<StatisticType, float> _completeBuffer = new();

	public StatisticsSet() {
		RecalculateBuffer();
	}

	public void RecalculateStuff(ISet<StatisticEntry> entries) {
		_stuff_flat.Clear();
		_stuff_mult.Clear();
		foreach(var entry in entries) {
			if(entry.IsMultiplicative) {
				_stuff_mult.AddOrCreate(entry.Type, entry.Value);
			} else {
				_stuff_flat.AddOrCreate(entry.Type, entry.Value);
			}
		}
		RecalculateBuffer();
	}

	private void RecalculateBuffer() {
		_completeBuffer.Clear();
		foreach(StatisticType type in System.Enum.GetValues(typeof(StatisticType))) {
			_completeBuffer.Add(type,
					(_stuff_flat.TryGet(type, 0) /* + other flats*/) * (_stuff_mult.TryGet(type, 1f)/* + other mults*/)
			);
		}
	}

	public float Get(StatisticType type) {
		return _completeBuffer[type];
	}

	public float this[StatisticType key] {
		get => Get(key);
	}


}