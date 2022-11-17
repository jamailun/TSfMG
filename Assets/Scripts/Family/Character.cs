using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Character : IStateSerializable<CharacterState> {
	
	public string Name { get; }
	public bool IsMale { get; }
	public float LifeExpectancy { get; private set; }
	public float Age { get; private set; }
	public bool IsDead { get; private set; }

	// stats !!

	public List<Curse> curses;

	public Character(CharacterState data) {
		Name = data.name;
		Age = data.age;
		IsMale = data.isMale;
		LifeExpectancy = data.lifeExpectancy;
		IsDead = Age > LifeExpectancy;
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
			curses = this.curses.Select(cs => cs.Serialize()).ToArray()
			//TODO: add les trucs qui arriveront ensuite.
		};
	}

	public override string ToString() {
		return "Character{name="+Name+", male="+IsMale+", age="+Age+(IsDead?", DEAD":"")+"}";
	}


}