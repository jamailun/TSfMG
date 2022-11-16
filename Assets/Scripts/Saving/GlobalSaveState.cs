[System.Serializable]
public struct GlobalSaveState {

	public int id;

	public float timePlayed; // seconds
	public long timeCreated; // sys time
	public long timeLastOpened; // sys time

	public FamilyState family;

}