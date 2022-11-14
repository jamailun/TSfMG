using System.Collections;
using UnityEngine;

[System.Serializable]
public struct GlobalSaveState {

	public int id;

	public long timePlayedSeconds;
	public long timeCreated;
	public long timeLastOpened;

	public int generationNumber;
	public string generationName;
	public CharacterState[] generationCharacters;

}