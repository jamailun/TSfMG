using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct CharacterState {

	public string name;
	public float age; // les characters vieillissent à la fin des runs == décorélé du temps réel => on stocke juste l'age brut
	public bool isMale;
	public float lifeExpectancy;
	//TODO: appearance
	public CurseState[] curses;
	//TODO: courbe de potentiel

	public StatisticsSetState currentStats;
	public StatisticsSetState growthStats;

	/// ========================================================

	public static CharacterState Generate(LanguageNames names, float age = -1f, float lifeExpectancyModifier = 1f) {
		bool isMale = Random.value < 0.5f;
		return GenerateWithSex(names, isMale, age, lifeExpectancyModifier);
	}
	public static CharacterState GenerateWithSex(LanguageNames names, bool isMale, float age = -1f, float lifeExpectancyModifier = 1f) {
		// TODO générer les curses aléatoirement
		if(age < 0f)
			age = Random.Range(15f, 30f);

		Dictionary<StatisticType, float> stats = new();
		stats[StatisticType.HealthRegen] = 0;
		stats[StatisticType.HealthMax] = Random.Range(50f, 200f);
		stats[StatisticType.ManaMax] = Random.Range(70f, 150f);
		stats[StatisticType.ManaOptimisation] = 0;
		stats[StatisticType.ManaPower] = Random.Range(5f, 10f);
		stats[StatisticType.Damages] = Random.Range(3f, 6f);

		Dictionary<StatisticType, float> growth = new();
		growth[StatisticType.HealthMax] = Random.Range(5f, 10f);
		growth[StatisticType.ManaMax] = Random.Range(2f, 15f);
		growth[StatisticType.ManaOptimisation] = Random.Range(0f, 2f);
		growth[StatisticType.ManaPower] = Random.Range(1f, 3f);
		growth[StatisticType.Damages] = Random.Range(1f, 3f);

		return new CharacterState() {
			name = names.Random(isMale),
			isMale = isMale,
			age = age,
			lifeExpectancy = lifeExpectancyModifier * Random.Range(50f, 65f),
			// Stats
			currentStats = StatisticsSetState.Serialize(stats),
			growthStats = StatisticsSetState.Serialize(growth),
		};
	}

}