using UnityEngine;

public class Respawner : MonoBehaviour {

	[SerializeField] private float damages = 10f;

	private void OnTriggerEnter2D(Collider2D collision) {
		var player = collision.GetComponent<PlayerEntity>();
		if(player) {
			// damage
			player.RespawnAndDamage(damages);
		}
	}

}