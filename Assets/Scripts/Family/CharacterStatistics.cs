using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatistics {

	private readonly Dictionary<StatisticType, float> statistics = new();
	private readonly Dictionary<StatisticType, float> growth = new();

	public int Level { get; private set; }
	public float Experience { get; private set; }
	public float PreviousExpRequired { get; private set; }
	public float NextExpRequired { get; private set; }

	public Dictionary<StatisticType, float> Statistics => statistics;
	public Dictionary<StatisticType, float> Growth => growth;

	private Character _char;

	public CharacterStatistics(Character character, CharacterState state) {
		this._char = character;

		state.currentStats.Fill(statistics);
		state.growthStats.Fill(growth);
	}

	public static float ExperienceForLevel(int level) {
		if(level <= 1)
			return 0f;
		return Mathf.Pow(4 * level, 3f);
	}

	public async void AddExperience(float amount) {
		Experience += amount;
		while(Experience >= NextExpRequired) {
			Level++;
			PreviousExpRequired = NextExpRequired;
			NextExpRequired = ExperienceForLevel(Level + 1);
			//TODO levelup effect
			await LevelUpOperation(Level);
		}
	}

	public async Task LevelUpOperation(int newLevel) {
		Debug.Log("NEW LEVEL : " + newLevel);

		// Add growth
		foreach(var k in growth.Keys)
			statistics[k] += growth[k];

		// Event
		_char.onLevelUpEvent?.Invoke(newLevel);
	}



}