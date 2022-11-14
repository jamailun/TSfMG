using System.Collections.Generic;
using UnityEngine;

public class LevelRoom : MonoBehaviour {

	public const float ROOM_SIZE_UNIT = 5f;

	[Header("Data")]
	[SerializeField] private Vector2Int size = new(2, 1);
	[SerializeField] private int _maximumAmount = 1;
	public int MaximumAmount => _maximumAmount;

	[SerializeField] private RoomDoor[] doors = new RoomDoor[0];
	private List<RoomDoorInstance> doorsInstances;

	#region Additional data

	private static ulong ID_PROVIDER = 0;
	private ulong _id = ID_PROVIDER++;
	public ulong ID => _id;

	private bool _isPrefab = true;
	public bool IsPrefab => _isPrefab;
	public void SetNotPrefab(LevelRoom parent) {
		_isPrefab = false;
		_id = parent._id;
		if(!parent.IsPrefab)
			Debug.LogError("Set " + this + " NOT prefab with parent " + parent + "... which is not a prefab...");
	}

	public int DoorsAmount => doors.Length;

	public Vector2Int Bounds => size;
	public float Width => size.x * ROOM_SIZE_UNIT;
	public float Height => size.y * ROOM_SIZE_UNIT;
	public Vector2 Center => new(Width / 2f, Height / 2f);
	public Rect RealBounds => new(transform.position.x - (Width / 2f), transform.position.y - (Height / 2f), Width, Height);

	public bool IsSpawn => SpawnPoint != null;
	public EntryDoor SpawnPoint => GetComponentInChildren<EntryDoor>();
	public bool IsExit => ExitPoint != null;
	public ExitDoor ExitPoint => GetComponentInChildren<ExitDoor>();

	#endregion

	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.Get2dPos(), new(Width, Height));

		Gizmos.color = Color.blue;
		foreach(var door in doors) {
			var origin = door.GetLineSource(size);
			Gizmos.DrawLine(transform.Get2dPos() + origin, transform.Get2dPos() + origin + door.GetLineSize());
		}
	}

	#region Doors obtention

	public List<RoomDoor> GetDoors() {
		return new(doors);
	}

	public struct DoorMatchResult {
		public RoomDoorInstance prefabDoor;
		public RoomDoorInstance alreadyExisting;

		public Vector3 GetFuturePosition() {
			return alreadyExisting.GetPosition() - prefabDoor.GetLocalPosition();
		}
	}

	public List<DoorMatchResult> DoorMatches(List<RoomDoorInstance> otherDoors) {
		List<DoorMatchResult> list = new();
		foreach(var door in GetDoorsInstances()) {
			foreach(var other in otherDoors) {
				if(door.DoorData.DoorsMatch(other.DoorData)) {
					list.Add(new() { prefabDoor = door, alreadyExisting = other });
				}
			}
		}
		return list;
	}

	public List<RoomDoorInstance> GetDoorsInstances() {
		if(doorsInstances == null) {
			doorsInstances = new();
			foreach(var d in doors) {
				doorsInstances.Add(new(this, d));
			}
		}
		return doorsInstances;
	}

	#endregion

	public override bool Equals(object obj) => ((obj is LevelRoom other) && Equals(other));

	public bool Equals(LevelRoom r) {
		return r.ID == ID;
	}

	public override int GetHashCode() {
		return (int) ID;
	}

	public override string ToString() {
		return "LevelRoom(\"" + name + "\", id=" + ID + ", prefab="+IsPrefab+")";
	}
}