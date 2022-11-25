using UnityEngine;

public class CameraFollow : SingletonBehaviour<CameraFollow> {

	[Tooltip("The offset of the camera")]
	[SerializeField] private Vector3 offset = new(0, 0, -10);

	[Tooltip("The smoothing time required for damping.")]
	[SerializeField] private float smoothTime = 0.25f;

	private Vector3 _velocity;

	public Transform Target { get; set; }

	public override bool DontDestroyObjectOnLoad => false;

	void FixedUpdate() {
		if(Target == null)
			return;

		Vector3 targetPos = Target.position + offset;
		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
	}

}