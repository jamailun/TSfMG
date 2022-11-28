using UnityEngine;
using System.Collections.Generic;

public class SlimeMonster : MonsterEntity {

	[Header("Tentacles")]
	[SerializeField] private float _minLengthTentacle = 0.2f;
	[SerializeField] private float _maxLengthTentacle = 2f;
	[SerializeField] [Range(10, 100)] private int _tentaclesScanAmount = 12;
	[SerializeField] [Range(1, 10)] private int _maximumTentacles = 3;
	[SerializeField] private LayerMask _tentacleLayer;
	[SerializeField] private float _tentacleRefreshTime = 0.5f;
	[SerializeField] [Range(0.01f, 1f)] private float _tentaclePrivateRadius = 0.1f;
	[SerializeField] [Range(0f, 10f)] private float _customSpringForce = 3f;
	[SerializeField] private Rope _ropePrefab;

	[Header("Movement")]
	[SerializeField] [Range(0f, 1f)] private float _movementCooldown = 0.5f;
	[SerializeField] [Range(0f, 20f)] private float _movementForce = 0.1f;

	[Header("Anti-stuck")]
	[SerializeField] private bool _antiStuck_enabled = true;
	[SerializeField] [Range(0.01f, 1f)] private float _antiStuck_refresh = 0.2f;
	[SerializeField] [Range(0.01f, 1f)] private float _antiStuck_triggerDelta = 0.02f;
	[SerializeField] [Range(0f, 10f)] private float _antiStuck_inertRadius = 3f;
	[SerializeField] [Range(0f, 10f)] private float _antiStuck_force = 0.2f;

	[Header("Debug")]
	[SerializeField] private bool _debug_tentacles = false;
	[SerializeField] private bool _spawnRopes = true;
	[SerializeField] private Transform _target;
	[SerializeField] private bool useCustomSprings = false;

	// fields
	private Vector2 _targetDirection;
	private readonly List<Tentacle> tentacles = new();

	private float _nextMovement;
	private float _nextTentacleRefreshTime;
	private Rigidbody2D _rb;

	private Vector3 _antiStuck_position;
	private float _antiStuck_next;

	private enum State {
		Wainting,
		Moving_Toward,
		Attacking
	}

	protected override void AfterInit() {
		base.AfterInit();
		foreach(var c in GetComponents<SpringJoint2D>())
			Destroy(c);
		_rb = GetComponent<Rigidbody2D>();
	}

	private void OnDrawGizmos() {
		if(!_debug_tentacles)
			return;

		var pos = transform.position;
		// Tentacles
		Gizmos.color = Color.gray;
		if(_tentaclesScanAmount < 1)
			_tentaclesScanAmount = 1;
		float deltaTh = 2 * Mathf.PI / _tentaclesScanAmount;
		for(float th = 0f; th < 2 * Mathf.PI; th += deltaTh) {
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

		// Real tentacles
		foreach(var t in tentacles) {
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.position, t.Position);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(t.Position, _tentaclePrivateRadius);
			Gizmos.color = Color.black;
			Gizmos.DrawLine(t.Position, t.Position + t.Direction.ToVec3() * 0.5f);
		}

		// inert antistuck
		if(_antiStuck_enabled) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(pos, _antiStuck_inertRadius);
			Gizmos.color = new Color(.8f, .35f, .35f);
			Gizmos.DrawWireSphere(pos, _antiStuck_triggerDelta);
		}
	}
	
	protected override void EntityUpdate() {
		if(!_target)
			return;

		// Mouvement vers la cible.
		_targetDirection = (_target.position - transform.position).ToVec2().normalized;
		if(Time.time >= _nextMovement) {
			_nextMovement = Time.time + _movementCooldown;
			float mult = (1f / _movementCooldown);
			_rb.AddForce(_movementForce * mult * _targetDirection, ForceMode2D.Impulse);
		}
		
		// Refresh des tentacles
		if(Time.time >= _nextTentacleRefreshTime) {
			_nextTentacleRefreshTime = Time.time + _tentacleRefreshTime;
			// on créer des tentacules si il faut en créer
			if(tentacles.Count < _maximumTentacles) {
				CreateTentacle(true);
			}

			// on supprime les tentacles trop loin / où le dot product est naze
			tentacles.RemoveAll(t => {
				bool r = t.ShouldBeRemoved();
				if(r)
					t.DisableSpring();
				return r;
			});
		}

		// on regarde si on est coincé
		if(_antiStuck_enabled && Time.time >= _antiStuck_next) {
			_antiStuck_next = Time.time + _antiStuck_refresh;

			float distance_target = Vector3.Distance(transform.position, _target.position);
			// only apply the unstuk if far from the target.
			if(distance_target <= _antiStuck_inertRadius)
				return;

			float distance_last = Vector3.Distance(transform.position, _antiStuck_position);
			
			if(distance_target > 1f && distance_last <= _antiStuck_triggerDelta) {
				// si on a pas bougé par rapport au dernier check, essaye de se débloquer
				//_rb.AddForce(_antiStuck_force * -_targetDirection, ForceMode2D.Impulse);
				_rb.AddForce(_antiStuck_force * Random.insideUnitCircle.normalized, ForceMode2D.Impulse);
				//Debug.LogWarning("PAF try to move");
			}
			_antiStuck_position = transform.position;
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
		// Direction of the tentacle
		var dir = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta)).normalized;

		// We want the tentacle to be 
		if(dotProduct && Vector3.Dot(dir, _targetDirection) <= 0.01f)
			return null;
		
		// Raycast
		var r = Physics2D.Raycast(pos, dir, _maxLengthTentacle, _tentacleLayer);
		if(r && r.distance >= _minLengthTentacle) {

			// Evite de mettre des tentacules au même endroits
			if(tentacles.Find(t => Vector3.Distance(t.Position, r.point) <= _tentaclePrivateRadius) != null)
				return null;

			// Create the tentacle
			var t = new Tentacle(this, r.transform, r.point);
			tentacles.Add(t);
			return t;
		}
		return null;
	}

	public class Tentacle {
		private readonly Transform target;
		private Vector3 absoluteAnchor;
		private readonly Vector3 relativeAnchor;
		private readonly SlimeMonster slime;

		private SpringJoint2D _springUnity;
		private Spring2D _spring;

		private Rope ropeDisplay;
		public Tentacle(SlimeMonster owner, Transform target, Vector2 absoluteAnchor) {
			this.slime = owner;
			this.target = target;
			this.relativeAnchor = absoluteAnchor.ToVec3() - target.position;
			this.absoluteAnchor = absoluteAnchor;
			EnableSpring();
		}

		public void DisableSpring() {
			if(_spring)
				Destroy(_spring);
			_spring = null;
			if(_springUnity)
				Destroy(_springUnity);
			_springUnity = null;
			if(ropeDisplay)
				Destroy(ropeDisplay.gameObject);
		}

		public void EnableSpring() {
			if(_spring)
				return;
			if(slime.useCustomSprings) {
				// custom spring
				_spring = slime.gameObject.AddComponent<Spring2D>();
				_spring._toMove = slime._rb;
				_spring._anchor = absoluteAnchor;
				_spring._force = slime._customSpringForce;
			} else {
				// Unity spring
				_springUnity = slime.gameObject.AddComponent<SpringJoint2D>();
				_springUnity.connectedAnchor = absoluteAnchor;
				_springUnity.dampingRatio = 0.1f;
				_springUnity.distance = (slime._maxLengthTentacle + slime._minLengthTentacle) / 2.2f;
			}

			if(slime._spawnRopes && slime._ropePrefab) {
				// create the rope
				ropeDisplay = Instantiate(slime._ropePrefab, slime.transform);
				ropeDisplay.Init(slime.transform.position, Position);
				// add contraints to the rope
				ropeDisplay.AddConstraint(ropeDisplay.FirstIndex, () => slime.transform.position);
				ropeDisplay.AddConstraint(ropeDisplay.LastIndex, () => Position);
			}
		}

		// Grâce à ce truc, on peut avoir un slime évoluant sur des surfaces qui se déplacent !
		public Vector3 Position => target.position + relativeAnchor;

		public float Length => Vector3.Distance(Position, slime.transform.position);

		public Vector2 Direction => (Position - slime.transform.position).ToVec2().normalized;

		public Vector3 SlimePosition => slime.transform.position;

		public override string ToString() {
			return "Tentacle{to=" + target.name + ",abs_pos=" + Position + ", length=" + Length + "}";
		}
		public bool ShouldBeRemoved() {
			float l = Length;
			// trop loin, trop près
			return
				// Si on est trop loin
				l > slime._maxLengthTentacle
				// Si on est trop près
				|| l < slime._minLengthTentacle
				// Si on est derrière
				|| Vector3.Dot(Direction, slime._targetDirection) < -0.15f;
		}
	}

}