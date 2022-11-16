using UnityEngine;

[System.Serializable]
public struct CharacterState {

	public string name;
	public float age; // les characters vieillissent à la fin des runs == décorélé du temps réel.
	public bool isMale;
	public float lifeExpectancy;
	//TODO: appearance
	//TODO: curses
	//TODO: stats
	//TODO: courbe de potentiel

	public static CharacterState Generate(LanguageNames names, bool isMale, float age = 1f, float lifeExpectancyModifier = 1f) {
		return new CharacterState() {
			name = names.Random(isMale),
			isMale = isMale,
			age = age,
			lifeExpectancy = lifeExpectancyModifier * Random.Range(50f, 65f)
		};
	}


}