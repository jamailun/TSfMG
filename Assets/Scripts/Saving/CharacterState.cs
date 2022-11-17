using UnityEngine;

[System.Serializable]
public struct CharacterState {

	public string name;
	public float age; // les characters vieillissent à la fin des runs == décorélé du temps réel.
	public bool isMale;
	public float lifeExpectancy;
	//TODO: appearance
	public CurseState[] curses;
	//TODO: stats
	//TODO: courbe de potentiel

	public static CharacterState Generate(LanguageNames names, float age = -1f, float lifeExpectancyModifier = 1f) {
		bool isMale = Random.value < 0.5f;
		return GenerateWithSex(names, isMale, age, lifeExpectancyModifier);
	}
	public static CharacterState GenerateWithSex(LanguageNames names, bool isMale, float age = -1f, float lifeExpectancyModifier = 1f) {

		// TODO générer les curses aléatoirement
		if(age < 0f)
			age = Random.Range(15f, 30f);

		return new CharacterState() {
			name = names.Random(isMale),
			isMale = isMale,
			age = age,
			lifeExpectancy = lifeExpectancyModifier * Random.Range(50f, 65f)
		};
	}

}