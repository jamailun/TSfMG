using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

    private LineRenderer lineRenderer;
    private readonly List<RopeSegment> ropeSegments = new();
    [Tooltip("Size of each segment")] [SerializeField] private float ropeSegLen = 0.25f;
    [Tooltip("Amount of segments")] [SerializeField] private int segmentLength = 35;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private Vector2 gravity = new(0, -1.5f);

    [SerializeField] private bool startAsDemo = false;

    public int FirstIndex => 0;
    public int LastIndex => segmentLength - 1;

    public delegate Vector3 Vector3Provider();
    private readonly Dictionary<int, Vector3Provider> constraints = new();

    // Use this for initialization
    void Start() {
        // init line renderer
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.lineRenderer.startWidth = lineWidth;
        this.lineRenderer.endWidth = lineWidth;

        if(startAsDemo) {
            // Démo. Create the points
            Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            for(int i = 0; i < segmentLength; i++) {
                this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
                ropeStartPoint.y -= ropeSegLen;
            }

            // and follow the mouse
            AddConstraint(0, () => Camera.main.ScreenToWorldPoint(Input.mousePosition));
            AddConstraint(ropeSegments.Count - 1, () => transform.position);
        }
    }

	public void Init(Vector3 from, Vector3 to) {
        Vector2 dir = (to - from).normalized;
        float delta = Vector3.Distance(from, to) / segmentLength;
        Vector2 pos = from;

        for(int i = 0; i < segmentLength; i++) {
            this.ropeSegments.Add(new RopeSegment(pos));
            pos += (delta * dir);
        }
    }

	// Update is called once per frame
	void Update() {
        this.DrawRope();
    }

    private void FixedUpdate() {
        this.Simulate();
    }

    private void Simulate() {
        // SIMULATION
        for(int i = 1; i < this.segmentLength; i++) {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += gravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for(int i = 0; i < 50; i++) {
            this.ApplyConstraint();
        }
    }

    public void AddConstraint(int index, Vector3Provider provider) {
        constraints[index] = provider;
    }
    private void ConstraintPoint(int index, Vector2 position) {
        RopeSegment segment = this.ropeSegments[index];
        segment.posNow = position;
        this.ropeSegments[index] = segment;
    }

    private void ApplyConstraint() {
        // fixed points
        foreach(var e in constraints) {
            ConstraintPoint(e.Key, e.Value());
        }

        for(int i = 0; i < this.segmentLength - 1; i++) {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if(dist > ropeSegLen) {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            } else if(dist < ropeSegLen) {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if(i != 0) {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            } else {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope() {
#if UNITY_EDITOR
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
#endif

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for(int i = 0; i < this.segmentLength; i++) {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos) {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}
