using UnityEngine;

public class MonoCameraHook : MonoBehaviour {

	private void Start() {
		CameraFollow.Instance.Target = transform;
	}
}