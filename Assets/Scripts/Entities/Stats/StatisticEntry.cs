[System.Serializable]
public class StatisticEntry {

	public StatisticType Type { get; }
	public float Value { get; }
	public bool IsMultiplicative { get; }

	public StatisticEntry(StatisticType type, float value, bool isMult) {
		this.Type = type;
		this.Value = value;
		this.IsMultiplicative = isMult;
	}

}