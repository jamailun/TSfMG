[System.Serializable]
public struct CharacterState {

	public int id;
	public string name;
	public long birthTime;
	public bool isMale;

	public static CharacterState Generate(int id, LanguageNames names, bool isMale) {
		return new CharacterState() {
			id = id,
			name = names.Random(isMale),
			isMale = isMale,
			birthTime = SaveManager.NOW
		};
	}


}