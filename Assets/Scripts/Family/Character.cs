using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Character : IStateSerializable<CharacterState> {
	
	public string Name { get; }
	public bool IsMale { get; }
	public float LifeExpectancy { get; private set; }
	public float Age { get; private set; }
	public bool IsDead { get; private set; }

	// ----- TODO
	// stats !!

	public int Level => _statistics.Level;
	public float Experience => _statistics.Experience;

	public List<Curse> curses;

	// ------------
	private readonly CharacterStatistics _statistics;
	public Dictionary<StatisticType, float> CurrentStatistics => _statistics.Statistics;
	public OnLevelUpEvent onLevelUpEvent;
	// 

	public Character(CharacterState data) {
		Name = data.name;
		Age = data.age;
		IsMale = data.isMale;
		LifeExpectancy = data.lifeExpectancy;
		IsDead = Age > LifeExpectancy;

		_statistics = new(this, data);
	}

	public void YearsPassed(float years) {
		Age += years;
		if(Age > LifeExpectancy) {
			IsDead = true;
		}
	}

	public CharacterState Serialize() {
		return new() {
			name = Name,
			age = Age,
			lifeExpectancy = LifeExpectancy,
			isMale = IsMale,
			curses = this.curses.Select(cs => cs.Serialize()).ToArray(),
			//TODO: add les trucs qui arriveront ensuite.

			currentStats = StatisticsSetState.Serialize(_statistics.Statistics),
			growthStats = StatisticsSetState.Serialize(_statistics.Growth),
		};
	}

	public override string ToString() {
		return "Character{name="+Name+", male="+IsMale+", age="+Age+(IsDead?", DEAD":"")+"}";
	}


	public delegate void OnLevelUpEvent(int newLevel);

}