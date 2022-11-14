using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	protected static Vector3 PLAN = new(){ x = 1, y = 1, z = 0 };

	[SerializeField] protected bool damagePlayers = true;
	[SerializeField] protected bool damageMonsters = false;

	[HideInInspector] public float Damages { get; private set; }

	[Tooltip("The duration of the projectile (in seconds)")]
	[SerializeField] protected float lifeDuration = 5f;

	[Tooltip("The speed of the projectile")]
	[SerializeField] protected float speed = 100f;

	protected Transform parent;

	// Called by the server.
	public virtual void Init(Vector3 sourcePosition, Vector2 direction, Transform realParent) {
		this.parent = realParent;

		// Set position and rotation
		transform.SetPositionAndRotation(sourcePosition, Quaternion.identity);

		Vector3 dir = direction * PLAN;

		// set direction
		GetComponent<Rigidbody2D>().AddForce(dir.normalized * speed);
	}

	private void Start() {
		OnStart();
	}

	protected virtual void OnStart() {
		Destroy(gameObject, lifeDuration);
	}

	protected virtual void OnTrigger(Hurtbox box) {
		if(damagePlayers && box.Owner.IsPlayer || damageMonsters && box.Owner.IsMonster) {
			box.Damage(this);
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		var box = collision.GetComponent<Hurtbox>();
		if(box != null) {
			Debug.Log("SERVER: Projectile triggeredded on " + collision.gameObject.name);
			OnTrigger(box);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if(collision == null || (parent != null && (collision.transform == parent || collision.transform.IsChildOf(parent))))
			return;
		Debug.Log("SERVER: Projectile collided with " + collision.gameObject.name);
	}

}
