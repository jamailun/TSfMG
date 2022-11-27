using UnityEngine;
using System.Collections.Generic;

public class SlimeMonster : MonsterEntity {

	[SerializeField] private float _minLengthTentacle = 0.2f;
	[SerializeField] private float _maxLengthTentacle = 2f;
	[SerializeField] [Range(10, 100)] private int _tentaclesScanAmount = 12;
	[SerializeField] [Range(1, 10)] private int _maximumTentacles = 3;
	[SerializeField] private LayerMask _tentacleLayer;
	[SerializeField] private float _tentacleRefreshTime = 0.5f;
	[SerializeField] [Range(0.01f, 1f)] private float _tentaclePrivateRadius = 0.1f;

	// DEBUG
	[SerializeField] private Transform _target;

	// fields
	private Vector2 _targetDirection;
	private readonly List<Tentacle> tentacles = new();

	private float _nextTentacleRefreshTime;

	private void OnDrawGizmos() {
		// Tentacles
		Gizmos.color = Color.gray;
		if(_tentaclesScanAmount < 1)
			_tentaclesScanAmount = 1;
		float deltaTh = 2 * Mathf.PI / _tentaclesScanAmount;
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
			Gizmos.DrawLine(pos, pos + 0.5f * _targetDirection.ToVec3());
		}

		foreach(var t in tentacles) {
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.position, t.Position);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(t.Position, _tentaclePrivateRadius);
			Gizmos.color = Color.black;
			Gizmos.DrawLine(t.Position, t.Position + t.Direction.ToVec3() * 0.5f);
		}
	}


	protected override void EntityUpdate() {
		if(_target) {
			_targetDirection = (_target.position - transform.position).ToVec2().normalized;
			//if gounded, go towards it

			// pas de tentacule : on va en créer

			if(Time.time >= _nextTentacleRefreshTime) {
				// on créer des tentacules si il faut en créer
				if(tentacles.Count < _maximumTentacles) {
					var t = CreateTentacle(true);
					if(t != null)
						Debug.Log("new tentacule !");
				}

				// on supprime les tentacles trop loin / où le dot product est naze
				int cnt = tentacles.RemoveAll(t => {
					float l = t.Length;
					return l > _maxLengthTentacle || l < _minLengthTentacle;
				});
				if(cnt > 0)
					Debug.LogWarning("Removed " + (cnt) + " tentacle(s) ! (DISTANCE)");
				cnt = tentacles.RemoveAll(t => Vector3.Dot(t.Direction, _targetDirection) < -0.15f);
				if(cnt > 0)
					Debug.LogWarning("Removed " + (cnt) + " tentacle(s) ! (ANGLE)");
			}
		}
	}

	private bool _tentacleInterationPositive = true;
	private Tentacle CreateTentacle(bool dotProduct) {
		// variable to not recaulate everything all the time
		float deltaTh = 2 * Mathf.PI / _tentaclesScanAmount;
		var pos = transform.position.ToVec2();

		// invert the next iteration direction
		_tentacleInterationPositive = !_tentacleInterationPositive;

		// loop over the slime
		if(_tentacleInterationPositive) {
			for(float th = 0f; th < 2 * Mathf.PI; th += deltaTh) {
				var tentacle = TryCreateTentacleIteration(dotProduct, th, pos);
				if(tentacle != null)
					return tentacle;
			}
		} else {
			for(float th = 2 * Mathf.PI; th > 0; th -= deltaTh) {
				var tentacle = TryCreateTentacleIteration(dotProduct, th, pos);
				if(tentacle != null)
					return tentacle;
			}
		}

		return null;
	}

	private Tentacle TryCreateTentacleIteration(bool dotProduct, float theta, Vector2 pos) {
		var dir = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta)).normalized;
		// We want the tentacle to be 
		if(dotProduct && Vector3.Dot(dir, _targetDirection) <= 0.01f)
			return null;


		var r = Physics2D.Raycast(pos, dir, _maxLengthTentacle, _tentacleLayer);
		if(r && r.distance >= _minLengthTentacle) {

			// Evite de mettre des tentacules au même endroits
			if(tentacles.Find(t => Vector3.Distance(t.Position, r.point) <= _tentaclePrivateRadius) != null) {
				return null;
			}


			var t = new Tentacle(this, r.transform, r.point);
			tentacles.Add(t);
			Debug.LogWarning("new tentacle = " + t);

			return t;
		}
		return null;
	}

	private class Tentacle {
		private readonly Transform target;
		private readonly Vector3 relativeAnchor;
		private readonly SlimeMonster slime;
		public Tentacle(SlimeMonster owner, Transform target, Vector2 absoluteAnchor) {
			this.slime = owner;
			this.target = target;
			this.relativeAnchor = absoluteAnchor.ToVec3() - target.position;
		}

		// Grâce à ce truc, on peut avoir un slime évoluant sur des surfaces qui se déplacent !
		public Vector3 Position => target.position + relativeAnchor;

		public float Length => Vector3.Distance(Position, slime.transform.position);

		public Vector2 Direction => (Position - slime.transform.position).ToVec2().normalized;

		public override string ToString() {
			return "Tentacle{to=" + target.name + ",abs_pos=" + Position + ", length=" + Length + "}";
		}
	}

}