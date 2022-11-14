using System.Collections;
using UnityEngine;

public class ExitDoor : MapInteractable {

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, Radius);
	}

	public override void Interract() {
		throw new System.NotImplementedException();
	}

}