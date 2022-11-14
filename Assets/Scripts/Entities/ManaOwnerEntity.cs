using UnityEngine;

public abstract class ManaOwnerEntity : LivingEntity {

    [SerializeField] protected float _maxMana = 50f;
    public float MaxMana => _maxMana;
    public float Mana { get; private set; }

	protected override void InitEntity() {
		base.InitEntity();
        SetFullMana();
	}

	public void SetFullMana() {
        Mana = _maxHealth;
	}

    public float UseMana(float amount) {
        if(IsDead)
            return 0;
        // Get mana reduction ?
        Mana -= amount;
        ManaChanged(-amount);
        return amount;
    }


    public void RegenMana(float amount) {
        if(IsDead)
            return;

        // Heal effect
        Mana += amount;
        if(Mana > MaxMana)
            Mana = MaxMana;

        ManaChanged(amount);
    }

    protected virtual void ManaChanged(float delta) { }

}
