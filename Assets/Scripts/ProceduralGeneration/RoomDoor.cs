using System.Collections;
using UnityEngine;

[System.Serializable]
public struct RoomDoor {

	[System.Serializable]
	public enum RoomDoorSide {
		Left,
		Right,
		Top,
		Bottom
	}
	public static RoomDoorSide GetOppositeSide(RoomDoorSide side) {
		return side switch {
			RoomDoorSide.Left => RoomDoorSide.Right,
			RoomDoorSide.Right => RoomDoorSide.Left,
			RoomDoorSide.Top => RoomDoorSide.Bottom,
			RoomDoorSide.Bottom => RoomDoorSide.Top,
			_ => 0
		};
	}

	[SerializeField] private RoomDoorSide side;
	[SerializeField] private int size;
	[SerializeField] private int delta;

	public float GetX(int width) {
		float h = width * LevelRoom.ROOM_SIZE_UNIT / 2f;
		if(side == RoomDoorSide.Left)
			return -h;
		if(side == RoomDoorSide.Right)
			return h;
		return delta * LevelRoom.ROOM_SIZE_UNIT - h;
	}

	public float GetY(int height) {
		float h = height * LevelRoom.ROOM_SIZE_UNIT / 2f;
		if(side == RoomDoorSide.Bottom)
			return -h;
		if(side == RoomDoorSide.Top)
			return h;
		return delta * LevelRoom.ROOM_SIZE_UNIT - h;
	}

	public Vector2 GetLineSource(Vector2Int bounds) {
		return new Vector2(GetX(bounds.x), GetY(bounds.y));
	}

	public Vector2 GetLineSize() {
		if(side == RoomDoorSide.Left || side == RoomDoorSide.Right)
			return new Vector2(0, size * LevelRoom.ROOM_SIZE_UNIT);
		return new Vector2(size * LevelRoom.ROOM_SIZE_UNIT, 0);
	}

	public bool DoorsMatch(RoomDoor door) {
		return GetOppositeSide(side) == door.side && door.size == size;
	}

	public override int GetHashCode() {
		return ((int)side * 1000) + (size * 100) + (delta * 10);
	}

}