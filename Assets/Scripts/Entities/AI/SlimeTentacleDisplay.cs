using System.Collections.Generic;
using UnityEngine;

public class SlimeTentacleDisplay : MonoBehaviour {

	private LineRenderer lineRenderer;
	private readonly List<SlimeTentacleSegment> tentacleSegments = new();

	[SerializeField] private float distanceBetweenSegments = 0.25f;
	[SerializeField] private float tentacleWidth = 0.1f;
	[SerializeField] private int segmentsCount = 35;

	private int FirstIndex => 0;
	private int LastIndex => segmentsCount - 1;

	private SlimeMonster.Tentacle tentacle;

	public void Init(SlimeMonster.Tentacle tentacle) {
		this.tentacle = tentacle;

		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.startWidth = tentacleWidth;
		lineRenderer.endWidth = tentacleWidth;

		Vector2 origin = tentacle.SlimePosition;
		Vector2 target = tentacle.Position;
		Vector2 dir = (target - origin).normalized;
		float delta = tentacle.Length / segmentsCount;
		Vector2 pos = origin;

		// 1er point == source, dernier = anchor
		for(int i = 0; i < segmentsCount; i++) {
			tentacleSegments.Add(new(pos));
			pos += (delta * dir);
		}

		Debug.Log("coucou, gravité = " + Physics2D.gravity);
	}

	private void Update() {
		Draw();
	}

	private void FixedUpdate() {
		Simulate();
		ApplyConstraints();
	}

	private void Simulate() {
		// calculate the position of points
		for(int i = 0; i < segmentsCount; i++) {
			SlimeTentacleSegment seg = this.tentacleSegments[i];
			Vector2 velocity = seg.posNow - seg.posOld;
			seg.posOld = seg.posNow;
			seg.posNow += velocity + (Physics2D.gravity * Time.fixedDeltaTime);
			this.tentacleSegments[i] = seg;
		}

		// CONSTRAINTS
		// first segment == slime, last segment == anchor.
		ConstraintPoint(FirstIndex, tentacle.SlimePosition);
		ConstraintPoint(LastIndex, tentacle.Position);

		for(int i = 0; i < 50; i++) {
		//	ApplyConstraints();
		}
	}

	private void ApplyConstraints() {
		// all points must have the same distance between them.
		for(int i = FirstIndex + 1; i < LastIndex; i++) {
			SlimeTentacleSegment previousPoint = this.tentacleSegments[i - 1];
			SlimeTentacleSegment currentPoint = this.tentacleSegments[i];

			float dist = (previousPoint.posNow - currentPoint.posNow).magnitude;
			float error = Mathf.Abs(dist - this.distanceBetweenSegments);

			Vector2 changeDir = Vector2.zero;
			if(dist > this.distanceBetweenSegments) {
				changeDir = (previousPoint.posNow - currentPoint.posNow).normalized;
			} else if(dist < this.distanceBetweenSegments) {
				changeDir = (currentPoint.posNow - previousPoint.posNow).normalized;
			}
				
			Vector2 changeAmount = changeDir * error;

			currentPoint.posNow += changeAmount;
			this.tentacleSegments[i] = currentPoint;
		}
	}

	private void ConstraintPoint(int index, Vector2 position) {
		SlimeTentacleSegment segment = this.tentacleSegments[index];
		segment.posNow = position;
		this.tentacleSegments[index] = segment;
	}

	private void Draw() {
#if UNITY_EDITOR
		// uniquement pour le debug :)
		lineRenderer.startWidth = tentacleWidth;
		lineRenderer.endWidth = tentacleWidth;
#endif

		Vector3[] positions = new Vector3[this.segmentsCount];
		for(int i = 0; i < segmentsCount; i++) {
			positions[i] = tentacleSegments[i].posNow;
		}
		lineRenderer.positionCount = positions.Length;
		lineRenderer.SetPositions(positions);

	}

	public struct SlimeTentacleSegment {
		public Vector2 posNow;
		public Vector2 posOld;
		public SlimeTentacleSegment(Vector2 pos) {
			this.posNow = pos;
			this.posOld = pos;
		}
	}
}