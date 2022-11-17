using System;
using System.Collections.Generic;

[Serializable]
public struct StatState {

	public StatisticType type;
	public float value;

}

[Serializable]
public struct StatisticsSetState {

	public StatState[] statistics;

	public void Fill(Dictionary<StatisticType, float> dict) {
		foreach(var s in statistics) {
			dict[s.type] = s.value;
		}
	}
	public static StatisticsSetState Serialize(Dictionary<StatisticType, float> dict) {
		StatisticsSetState stats = new();
		stats.statistics = new StatState[dict.Count];
		int i = 0;
		foreach(var s in dict.Keys) {
			stats.statistics[i++] = new() { type = s, value = dict[s] };
		}
		return stats;
	}

}