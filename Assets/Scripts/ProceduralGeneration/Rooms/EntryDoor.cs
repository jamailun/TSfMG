using System.Collections;
using UnityEngine;

public class EntryDoor : MonoBehaviour {

	public void Spawn(PlayerEntity playerPrefab) {
		var player = Instantiate(playerPrefab);
		player.transform.position = transform.position;
	}
	
}