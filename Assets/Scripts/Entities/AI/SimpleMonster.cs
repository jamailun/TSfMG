using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SimpleMonster : MonsterEntity {
	public override EntityType EntityType => EntityType.Monster;

	[Header("Simple movement settings")]
	[SerializeField]private bool _goingRight = true;
	public bool GoingRight => _goingRight;

	//[SerializeField] private Vector2 _delta;
	[SerializeField] private float _deltaRayHorizontal = 0.01f;
	[SerializeField] private Vector2 _raycastLength;
	[SerializeField] private LayerMask _groundLayer;
	private float RaycastLengthX => _raycastLength.x;
	private float RaycastLengthY => _raycastLength.y;
	private float DeltaX => _box.offset.x * _goingRight.ToMult();
	private float DeltaY => _box.offset.y;
	private float HalfSizeX => _box.size.x / 2f;
	private float HalfSizeY => _box.size.y / 2f;
	private Vector3 RaycastOriginBottom => transform.position + new Vector3(_goingRight.ToMult() * (HalfSizeX + DeltaX), DeltaY - HalfSizeY);
	private Vector3 RaycastOriginLatteral => transform.position + new Vector3(_goingRight.ToMult() * (HalfSizeX + DeltaX), 0);

	private BoxCollider2D _box;

	// panic mode == on est en train de tomber. Pour éviter de glitcher dans tous les sens, on se met en pause et on attend
	private bool _panicModeActive = false;
	private float _panicModeNext;

	protected override void AfterInit() {
		AssignBox();
		if(Random.Range(0, 2) == 0) {
			_goingRight = !_goingRight;
			UpdateGoingRight();
		}
	}

	private void OnDrawGizmosSelected() {
		AssignBox();
		Gizmos.color = Color.red;
		Gizmos.DrawLine(RaycastOriginBottom, RaycastOriginBottom + new Vector3(0, -RaycastLengthY));

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(RaycastOriginLatteral, RaycastOriginLatteral + new Vector3(RaycastLengthX * _goingRight.ToMult(), 0));
		Gizmos.DrawLine(RaycastOriginLatteral + new Vector3(0, -(HalfSizeY-_deltaRayHorizontal)), RaycastOriginLatteral + new Vector3(0, -HalfSizeY+ _deltaRayHorizontal) + new Vector3(RaycastLengthX * _goingRight.ToMult(), 0));
		Gizmos.DrawLine(RaycastOriginLatteral + new Vector3(0, (HalfSizeY-_deltaRayHorizontal)), RaycastOriginLatteral + new Vector3(0, HalfSizeY- _deltaRayHorizontal) + new Vector3(RaycastLengthX * _goingRight.ToMult(), 0));
	}

	protected override void EntityUpdate() {
		if(_panicModeActive) {
			// Ne rien faire avant le recalcul
			if(Time.time < _panicModeNext)
				return;
			// Recalcul
			if(ShoudChangeDirection()) {
				_panicModeNext = Time.time + 0.3f;
				return;
			} else {
				// fin du panic mode
				_panicModeActive = false;
			}
		}
		if(ShoudChangeDirection()) {
			_goingRight = !_goingRight;
			UpdateGoingRight();
			// test si on s'apprête à clignoter sévère.
			if(ShoudChangeDirection()) {
				// on va clignoter... on active le "panic mode"
				_panicModeActive = true;
				_panicModeNext = Time.time + 0.3f;
				return;
			}
		}
		Vector3 dir = new(_speed * _goingRight.ToMult(), 0);
		transform.position += Time.deltaTime * dir;
	}

	private readonly float[] DELTA_RAYCAST = { -1f, 0, 1f };

	// Fait en sorte que le monstre ne marque que sur les plateformes et se bloque pas dans les murs
	private bool ShoudChangeDirection() {
		// Test de vide sur les bords de la plateforme.
		RaycastHit2D result = Physics2D.Raycast(RaycastOriginBottom, new Vector2(0, -1f), RaycastLengthY, _groundLayer);
		if(!result) {
			//Debug.Log("vide !");
			return true;
		}

		// Test de mur en face de nous
		foreach(var d in DELTA_RAYCAST) {
			result = Physics2D.Raycast(RaycastOriginLatteral + new Vector3(0, d * (HalfSizeY-_deltaRayHorizontal)), new Vector2(_goingRight.ToMult(), 0), RaycastLengthX, _groundLayer);
			if(result) {
				//Debug.Log("wall !");
				return true;
			}
		}

		return false;
	}

	// Get the box collider.
	private void AssignBox() {
		if(!_box) {
			_box = GetComponent<BoxCollider2D>();
		}
	}

	private void UpdateGoingRight() {
		//Debug.Log("new direction ("+(_goingRight?"right":"left")+") !");
		SpriteRenderer.flipX = !_goingRight;
		Hurtbox.FlipX();
		_box.FlipX();
	}
	public void _debug_Flipx() {
		_goingRight = !_goingRight;
		UpdateGoingRight();
	}

}