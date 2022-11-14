using System.Collections;
using UnityEngine;

public abstract class MapInteractable : MonoBehaviour {

	[SerializeField] private float _radius = 1;
	public float Radius => _radius;

	private void Awake() {
		if(_radius < 0.1f) {
			_radius = 0.1f;
			Debug.LogWarning("MapInterractable " + name + " have a too small radius.");
		}
	}

	public abstract void Interract();

}