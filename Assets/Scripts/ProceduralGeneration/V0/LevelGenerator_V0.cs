using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DelaunatorSharp;

public class LevelGenerator_V0 : MonoBehaviour {

	private readonly List<RoomPlacement> rooms = new();
	private readonly List<IEdge> delaunatorEdges = new();
	private readonly List<IEdge> corridorsEdges = new();

	public bool DisplayGray { get; set; }
	private void Awake() {
		DisplayGray = true;
	}

	[Header("Room generation")]
	[Range(1, 100)] [SerializeField] private int roomsAmount = 20;
	[SerializeField] private Vector2 minRoomSize = new(0.3f, 0.2f);
	[SerializeField] private Vector2 maxRoomSize = new(0.6f, 0.4f);
	[SerializeField] private float roomSizeMultiplier = 2f;

	[Header("Room spacing")]
	[SerializeField] private float moveForce = 0.2f;
	[SerializeField] private int DEBUG_maxIterations_SOLVE = 1000;

	[Header("Room filter")]
	[Range(0.01f, 1f)] [SerializeField] private float percentFilterKeep = 0.3f;

	private void OnDrawGizmos() {
		if(DisplayGray) {
			// rooms
			foreach(var rp in rooms) {
				Gizmos.color = rp.Accepted ? Color.blue : rp.AcceptedSecondary ? Color.green : rp.Overlaps ? Color.red : Color.gray;
				Gizmos.DrawWireCube(rp.rect.center, rp.rect.size);
			}
			// edges
			foreach(var edge in delaunatorEdges) {
				Gizmos.color = (corridorsEdges.Contains(edge)) ? Color.cyan : Color.gray;
				Gizmos.DrawLine(new Vector2((float) edge.P.X, (float) edge.P.Y), new Vector2((float) edge.Q.X, (float) edge.Q.Y));
			}
		} else {
			// rooms
			Gizmos.color = Color.blue;
			foreach(var rp in rooms) {
				if(rp.Accepted || rp.AcceptedSecondary) {
					Gizmos.DrawWireCube(rp.rect.center, rp.rect.size);
				}
			}
			// edges
			Gizmos.color = Color.cyan;
			foreach(var edge in corridorsEdges) {
				Gizmos.DrawLine(new Vector2((float) edge.P.X, (float) edge.P.Y), new Vector2((float) edge.Q.X, (float) edge.Q.Y));
			}
		}

		// gravity
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(GravityCenter(), 0.05f);
	}

	public void Generate() {
		rooms.Clear();
		delaunatorEdges.Clear();
		corridorsEdges.Clear();

		for(int i = 0; i < roomsAmount; i++) {
			float w = Random.Range(minRoomSize.x, maxRoomSize.x) * roomSizeMultiplier;
			float h = Random.Range(minRoomSize.y, maxRoomSize.y) * roomSizeMultiplier;
			RoomPlacement rp = new(roomSizeMultiplier/3f, w, h, i);
			rooms.Add(rp);
		}
	}


	public void SeparateRooms() {
		int iterations = DEBUG_maxIterations_SOLVE;

		bool changed;
		do {
			changed = false;
			foreach(var room in rooms) {
				var center = GravityCenter();
				while(room.TestOverlaps(rooms, false)) {
					Vector2 dir = (room.rect.position - center).normalized;
					room.rect.position += dir * moveForce;
					if(--iterations <= 0)
						return;
					changed = true;
				}
			}
		} while(changed);
	}

	public void Filter() {
		int keeped = Mathf.Min(rooms.Count, (int) (percentFilterKeep * rooms.Count));
		rooms.Sort();
		for(int i = 0; i < keeped; i++) {
			rooms[i].Accepted = true;
		}
	}

	public void Triangulate() {
		var delaunator = new Delaunator(rooms.FindAll(r => r.Accepted).ToArray());
		// add edges
		delaunatorEdges.Clear();
		delaunatorEdges.AddRange(delaunator.GetEdges());
		// minimum spanning tree
		corridorsEdges.Clear();
		MinimumSpanningTree tree = new(delaunatorEdges, delaunator.Points.Length);
		var keeped = tree.Resolve();
		corridorsEdges.AddRange(keeped);
	}

	public void AddSecondaryRooms() {
		foreach(var room in rooms.FindAll(r => !r.Accepted && !r.AcceptedSecondary)) {
			room.AcceptedSecondary = false;
			foreach(var edge in corridorsEdges) {
				if(LineIntersectsRect(edge, room.rect)) {
					room.AcceptedSecondary = true;
				}
			}
		}
	}

	public void TriangulateSecondary() {
		var delaunator = new Delaunator(rooms.FindAll(r => r.Accepted || r.AcceptedSecondary).ToArray());
		// add edges
		delaunatorEdges.Clear();
		delaunatorEdges.AddRange(delaunator.GetEdges());
		// minimum spanning tree
		corridorsEdges.Clear();
		MinimumSpanningTree tree = new(delaunatorEdges, delaunator.Points.Length);
		var keeped = tree.Resolve();
		corridorsEdges.AddRange(keeped);
	}

	private Vector2 GravityCenter() {
		float cx = 0;
		float cy = 0;
		foreach(var room in rooms) {
			cx += room.rect.center.x;
			cy += room.rect.center.y;
		}
		float s = rooms.Count;
		return new(cx / s, cy / s);
	}

	public class RoomPlacement : System.IComparable<RoomPlacement>, IPoint {
		public Rect rect;
		public bool Overlaps { get; set; }
		public int Id { get; private set; }
		private readonly Vector2 dir;
		public bool Accepted { get; set; }
		public bool AcceptedSecondary { get; set; }
		public RoomPlacement(float mult, float w, float h, int id) {
			dir = Random.insideUnitCircle.normalized;
			rect = new Rect(-(w+dir.x*mult)/2f, -(h+dir.y*mult)/2f, w, h);
			Id = id;
			Overlaps = true;
			Accepted = false;
			AcceptedSecondary = false;
		}

		public void Move(float force) {
			rect.position += (dir * force);
		}

		public bool TestOverlaps(List<RoomPlacement> rps, bool stopFirst = true) {
			bool found = false;
			foreach(var rp in rps) {
				if(Id != rp.Id && rect.Overlaps(rp.rect)) {
					found = true;
					Overlaps = true;
					rp.Overlaps = true;
					if(stopFirst)
						return true;
				}
			}
			Overlaps = found;
			return found;
		}

		private float Area => rect.width * rect.height;

		public double X { get => rect.center.x; set => rect.center = new((float) value, rect.center.y); }
		public double Y { get => rect.center.y; set => rect.center = new(rect.center.x, (float) value); }

		public int CompareTo(RoomPlacement other) {
			return (int) (100f * (other.Area - Area));
		}
	}

	// UTILS

	public static bool LineIntersectsRect(IEdge edge, Rect r) {
		return LineIntersectsRect(new Vector2((float) edge.P.X, (float) edge.P.Y), new Vector2((float) edge.Q.X, (float) edge.Q.Y), r);
	}
	public static bool LineIntersectsRect(Vector2 p1, Vector2 p2, Rect r) {
		return LineIntersectsLine(p1, p2, new Vector2(r.x, r.y), new Vector2(r.x + r.width, r.y)) ||
			   LineIntersectsLine(p1, p2, new Vector2(r.x + r.width, r.y), new Vector2(r.x + r.width, r.y + r.height)) ||
			   LineIntersectsLine(p1, p2, new Vector2(r.x + r.width, r.y + r.height), new Vector2(r.x, r.y + r.height)) ||
			   LineIntersectsLine(p1, p2, new Vector2(r.x, r.y + r.height), new Vector2(r.x, r.y))
			   ;//|| (r.Contains(p1) && r.Contains(p2));
	}

	private static bool LineIntersectsLine(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2) {
		float q = (l1p1.y - l2p1.y) * (l2p2.x - l2p1.x) - (l1p1.x - l2p1.x) * (l2p2.y - l2p1.y);
		float d = (l1p2.x - l1p1.x) * (l2p2.y - l2p1.y) - (l1p2.y - l1p1.y) * (l2p2.x - l2p1.x);

		if(d == 0) {
			return false;
		}

		float r = q / d;

		q = (l1p1.y - l2p1.y) * (l1p2.x - l1p1.x) - (l1p1.x - l2p1.x) * (l1p2.y - l1p1.y);
		float s = q / d;

		if(r < 0 || r > 1 || s < 0 || s > 1) {
			return false;
		}

		return true;
	}

}