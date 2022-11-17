using UnityEngine;

public class Curse : IStateSerializable<CurseState> {

	public string name;

	public Curse() {
		// RANDOM
	}
	public Curse(CurseState data) {
		// FROM STATE
		name = data.name;
	}

	public CurseState Serialize() {
		return new CurseState() {
			name = name
		};
	}
}