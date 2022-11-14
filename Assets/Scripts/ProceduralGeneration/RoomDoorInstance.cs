using UnityEngine;

public class RoomDoorInstance {

	private static ulong ID_PROVIDER = 0;
	private readonly ulong ID = ID_PROVIDER++;

	private readonly LevelRoom room;
	public LevelRoom Room => room;
	private readonly RoomDoor doorData;
	public RoomDoor DoorData => doorData;

	public RoomDoorInstance(LevelRoom room, RoomDoor doorData) {
		this.room = room;
		this.doorData = doorData;
	}

	public Vector3 GetPosition() {
		return room.transform.position + GetLocalPosition();
	}
	public Vector3 GetLocalPosition() {
		return doorData.GetLineSource(room.Bounds).ToVec3();
	}

	public override int GetHashCode() {
		return (int) ID;
	}

	public override bool Equals(object obj) => (obj == this) || ((obj is RoomDoorInstance other) && Equals(other));

	public bool Equals(RoomDoorInstance other) {
		return other.ID == ID;
	}

}