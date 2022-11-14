using UnityEngine;

public class Hurtbox : MonoBehaviour {

	private LivingEntity _entity;
	public LivingEntity Owner => _entity;

	private void Start() {
		_entity = GetComponentInParent<LivingEntity>();
		if(_entity == null) {
			Debug.LogError("Hurtbox " + name + " could NOT get the parent entity.");
			enabled = false;
		}
	}

	public void Damage(Projectile proj) {
		Damage(proj.Damages);
	}

	public float Damage(Hitbox hitbox) {
		return Damage(hitbox.CurrentDamages);
	}

	public float Damage(float rawDamages) {
		return _entity.Damage(rawDamages);
	}

	public void FlipX() {
		UnityAugment.FlipX(GetComponent<Collider2D>());
	}

}