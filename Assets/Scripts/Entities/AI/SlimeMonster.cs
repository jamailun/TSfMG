using UnityEngine;

public class SlimeMonster : MonsterEntity {

	[SerializeField] private float _minLengthTentacle = 0.2f;
	[SerializeField] private float _maxLengthTentacle = 2f;
	[SerializeField] [Range(10, 100)] private int _tentaclesAmount = 12;

	[SerializeField] private Transform _target;

	private void OnDrawGizmos() {
		// Tentacles
		Gizmos.color = Color.red;
		if(_tentaclesAmount < 1)
			_tentaclesAmount = 1;
		float deltaTh = 2 * Mathf.PI / _tentaclesAmount;
		var pos = transform.position;
		for(float th = 0f; th < 2*Mathf.PI; th += deltaTh) {
			var dir = new Vector3(Mathf.Cos(th), Mathf.Sin(th));
			var start = pos + _minLengthTentacle * dir;
			var end = pos + _maxLengthTentacle * dir;
			Gizmos.DrawLine(start, end);
		}

		// Target
		if(_target) {
			Gizmos.color = Color.cyan;
			var dir = (_target.position - pos).normalized;
			Gizmos.DrawLine(pos, pos + 0.5f * dir);
		}
	}

	protected override void EntityUpdate() {
		
	}

}