using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	[SerializeField] private string pathToRooms = "Rooms";
	[SerializeField] private float minRoomDistance = 1f;
	[SerializeField] private bool ignoreRoomsAmountLimitations = false;
	[SerializeField] private int minAmountOfRooms = 8;
	[SerializeField] private PlayerEntity playerPrefab;

	public bool displayGizmos = false;

	private readonly List<LevelRoom> library = new();

	private readonly List<LevelRoom> existing = new();
	private readonly List<RoomDoorInstance> readyDoors = new();
	private EntryDoor entryDoor;
	private readonly List<ExitDoor> exitDoors = new();

	public void LoadLibrairy(bool force = false) {
		if(force || library.Count == 0) {
			library.Clear();
			library.AddRange(Resources.LoadAll<LevelRoom>(pathToRooms));
			int rm = library.RemoveAll(r => ! r.gameObject.activeSelf);
			if(rm > 0 && force)
				Debug.Log("Ignored " + rm + " room" + (rm > 1 ? "s" : "") + ".");
		}
	}

	private Rect DEBUG = new();
	private Rect DEBUG_2 = new();
	private void OnDrawGizmos() {
		if(!displayGizmos)
			return;
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(DEBUG.center, DEBUG.size);
		Gizmos.color = new(0.6f, 0.1f, 0.5f);
		Gizmos.DrawWireCube(DEBUG.center, new Vector2(DEBUG.width - minRoomDistance, DEBUG.height - minRoomDistance));
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(DEBUG_2.center, new Vector2(DEBUG_2.width, DEBUG_2.height));
	}

	public void PrintAcceptable() {
		string s = "";
		foreach(var r in library.FindAll(room => !room.IsSpawn && CanPlaceAnother(room)))
			s += r + "\n";
		Debug.Log("acceptable > " + s);
	}

	public void FullGeneration() {
		LoadLibrairy();
		Clean();
		int r = 0;
		while(AddRoom(false)) {
			r++;
		}
		Debug.Log("Generated " + r + " rooms.");
	}

	public void SpawnPlayer(PlayerEntity instance = null) {
		if(entryDoor) {
			if(instance) {
				entryDoor.MakesEnter(instance);
			} else if(playerPrefab) {
				entryDoor.Spawn(playerPrefab);
			} else {
				Debug.LogError("LEVELGENERATOR could not spawn player. No prefab/instance given.");
			}
		} else {
			Debug.LogError("Could not find any entry point...");
		}
	}

	public void Clean(bool deletePlayer = true) {
		var tempList = transform.Cast<Transform>().ToList();
		foreach(Transform child in tempList) {
			DestroyImmediate(child.gameObject);
		}
		if(deletePlayer)
			foreach(var p in FindObjectsOfType<PlayerEntity>())
				DestroyImmediate(p.gameObject);
		
		LoadLibrairy();
		existing.Clear();
		readyDoors.Clear();
		DEBUG = new();
		DEBUG_2 = new();
	}

	private bool CanPlaceAnother(LevelRoom roomPrefab) {
		return ignoreRoomsAmountLimitations || CountRoomsPlaces(roomPrefab) < roomPrefab.MaximumAmount;
	}

	public bool AddRoom(bool showEnd = true) {
		if(existing.Count == 0) {
			return AddFirstRoom();
		}
		if(readyDoors.Count == 0) {
			if(showEnd)
				Debug.LogWarning("Cannot add any room : no free door.");
			return false;
		}

		LevelRoom roomPrefab = null; // prefab to add
		LevelRoom.DoorMatchResult doorMatchResult = new() { };
		// only non-spawn and non-placed rooms.
		var acceptables = library.FindAll(room => !room.IsSpawn && CanPlaceAnother(room)).Shuffle();

		foreach(var prf in acceptables) {
			// get all doors that can accept this prefab 
			var allowed = prf.DoorMatches(readyDoors).Shuffle();
			if(allowed.Count == 0)
				continue;

			// now we take the first door which create NO collision.
			bool found = false;
			foreach(var result in allowed) {
				if(!CollidesWithARoom(prf, result.GetFuturePosition(), result.alreadyExisting.Room)) {

					// test si on finit pas la construction trop tôt.
					if(existing.Count + 1 < minAmountOfRooms && (readyDoors.Count + result.prefabDoor.Room.DoorsAmount - 2 < 1)) {
						Debug.Log("Trop tôt pour " + result.prefabDoor.Room + " !");
						continue;
					}

					doorMatchResult = result;
					roomPrefab = prf;
					found = true;
					break;
				}
			}

			if(found)
				break;
		}
		
		if( ! roomPrefab) {
			Debug.LogError("Could not find any suitable room or door.");
			return false;
		}

		// Create the room from the prefab
		var room = Instantiate(roomPrefab, transform);
		room.SetNotPrefab(roomPrefab);
		room.transform.position = doorMatchResult.GetFuturePosition();

		// update list of rooms and doors
		existing.Add(room);
		readyDoors.AddRange(room.GetDoorsInstances().FindAll(d => d.DoorData.GetHashCode() != doorMatchResult.prefabDoor.DoorData.GetHashCode()));
		int a = readyDoors.RemoveAll(d => d.Equals(doorMatchResult.alreadyExisting));

		return true;
	}

	private int CountRoomsPlaces(LevelRoom room) {
		int c = 0;
		foreach(var r in existing) {
			if(r.ID == room.ID)
				c++;
		}
		return c;
	}

	public bool CollidesWithARoom(LevelRoom room, Vector2 futurePosition, LevelRoom ignored) {
		float d = minRoomDistance / 2f;
		Rect rect = new(futurePosition.x - room.Center.x - d, futurePosition.y - room.Center.y - d, room.Width + minRoomDistance, room.Height + minRoomDistance);
		DEBUG = rect;
		foreach(var bound in existing) {
			if(bound.ID != ignored.ID) {
				if(bound.RealBounds.Overlaps(rect)) {
					DEBUG_2 = bound.RealBounds;
					Debug.Log("oh no, " + room + " overlaps with " + bound + " :( [ignored="+ignored+"]");
					return true;
				}
			}
		}
		return false;
	}

	private bool AddFirstRoom() {
		LevelRoom spawnPrefab = library.FindAll(lr => lr.IsSpawn).GetRandom();
		if(spawnPrefab == null) {
			Debug.LogError("No spawn room exists !");
			return false;
		}
		var spawn = Instantiate(spawnPrefab, transform);
		spawn.SetNotPrefab(spawnPrefab);
		spawn.transform.position = new Vector3(0, 0, 0);

		existing.Add(spawn);
		readyDoors.AddRange(spawn.GetDoorsInstances());

		entryDoor = spawn.SpawnPoint;

		return true;
	}


}