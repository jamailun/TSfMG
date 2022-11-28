using UnityEngine;

public class Spring2D : MonoBehaviour {

	public float _force = 12f;
	public float _minDistance = 0.5f;
	public Vector3 _anchor;
	public Rigidbody2D _toMove;

	private static Color _springColor = new(0.1f, 1f, 0.2f);

	private void OnDrawGizmos() {
		if(!_toMove) return;

		Gizmos.color = _springColor;
		Gizmos.DrawLine(_toMove.transform.position, _anchor);
	}

	private void FixedUpdate() {
		if( ! _toMove)
			return;
		var position = _toMove.transform.position;
		// Get distance and direction
		float distance = Vector3.Distance(position, _anchor);
		var dir = (_anchor - position).ToVec2().normalized;
		// add movement
		if(distance > _minDistance) {
			_toMove.AddForce(Time.fixedDeltaTime * _force * dir, ForceMode2D.Impulse);
		}
	}

}