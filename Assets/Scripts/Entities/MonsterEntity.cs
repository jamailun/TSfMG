using UnityEngine;
using TMPro;

public class MonsterEntity : LivingEntity {

	[SerializeField] private TMP_Text label;
	[SerializeField] protected float _speed = 5f;

	private void Start() {
		if(label)
			label.text = Name;
		AfterInit();
	}

	protected virtual void AfterInit() { }

	public override EntityType EntityType => EntityType.Monster;

}