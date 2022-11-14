using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UnityAugment {
	public static Vector2 Get2dPos(this Transform tr) {
		return new Vector2(tr.position.x, tr.position.y);
	}

	public static void DestroyChildren(this Transform transform) {
		var tempList = transform.Cast<Transform>().ToList();
		foreach(Transform child in tempList) {
			GameObject.Destroy(child.gameObject);
		}
	}
	public static IList<T> Shuffle<T>(this IList<T> list) {
		int n = list.Count;
		while(n > 1) {
			n--;
			int k = Random.Range(0, n);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
		return list;
	}

	public static T GetRandom<T>(this IList<T> list) {
		if(list.Count == 0)
			return default(T);
		return list[Random.Range(0, list.Count)];
	}

	public static Vector3 ToVec3(this Vector2 vec) {
		return new Vector3(vec.x, vec.y, 0);
	}
	public static Vector3 FlipX(this Vector3 vec, bool flip = true) {
		return new Vector3(flip ? -vec.x : vec.x, vec.y, 0);
	}
	public static Vector2 FlipX(this Vector2 vec, bool flip = true) {
		return new Vector2(flip ? -vec.x : vec.x, vec.y);
	}
	public static float ToMult(this bool b) {
		return b ? 1f : -1f;
	}
	public static void FlipX(this Collider2D collider, bool flip = true) {
		if(collider)
			collider.offset = collider.offset.FlipX(flip);
	}
	public static void AddOrCreate<T>(this Dictionary<T, float> dict, T k, float v) {
		if(dict.ContainsKey(k)) {
			dict[k] += v;
		} else {
			dict.Add(k, v);
		}
	}
	public static V TryGet<K, V>(this Dictionary<K, V> dict, K k, V v = default) {
		return dict.ContainsKey(k) ? dict[k] : v;
	}
	public static void AddRange<T>(this ISet<T> set, IEnumerable<T> elements) {
		foreach(var elem in elements)
			set.Add(elem);
	}
}