using System.Collections;
using UnityEngine;

public class SpellProduction : MonoBehaviour {

	[SerializeField] private Transform sourcePosition;

	[Header("Life duration")]
	[SerializeField] private bool lifeDurationEnabled = true;
	[SerializeField] private float lifeDuration = 0.5f;

	public Vector3 LocalPosition => sourcePosition == null ? new() : sourcePosition.localPosition.FlipX(flipX);
	private bool flipX = false;

	private void Start() {
		if(lifeDurationEnabled) {
			Destroy(gameObject, lifeDuration);
		}
	}

	public void SetFlipX() {
		flipX = true;
		foreach(var rdr in GetComponentsInChildren<SpriteRenderer>())
			rdr.flipX = true;
	}

	public virtual void SetActive(PlayerEntity player, bool flipX) {
		if(flipX)
			SetFlipX();

	}

}