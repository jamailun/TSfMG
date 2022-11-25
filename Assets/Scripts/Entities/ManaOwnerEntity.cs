using UnityEngine;

public abstract class ManaOwnerEntity : LivingEntity {

    [SerializeField] private float _maxMana = 50f;
    [SerializeField] protected float _manaRegen = 0f;
    public float MaxMana => _maxMana;
    public float Mana { get; private set; }
    protected BarUI _manaBar;

	protected override void InitEntity() {
		base.InitEntity();
        SetFullMana();
	}

    protected override void EntityUpdate() {
        RegenMana(_manaRegen * Time.deltaTime, false);
    }

    protected void SetMaxMana(float amount) {
        if(amount < 0)
            amount = 0;
        _maxMana = amount;
        if(Mana > _maxMana)
            Mana = _maxMana;
        if(_manaBar)
            _manaBar.Init(0, amount, Mana);
	}

    public void SetFullMana() {
        Mana = _maxMana;
        if(_manaBar)
            _manaBar.SetValue(Mana);
        ManaChanged(MaxMana);
    }

    public float UseMana(float amount) {
        if(IsDead)
            return 0;
        // Get mana reduction ?
        Mana -= amount;
        ManaChanged(-amount);

        if(_manaBar)
            _manaBar.SetValue(Mana);

        return amount;
    }

    public void RegenMana(float amount, bool showText = true) {
        if(IsDead || amount <= 0 || Mana >= MaxMana)
            return;

        // regen effect
        Mana += amount;
        if(Mana > MaxMana)
            Mana = MaxMana;

        if(_manaBar)
            _manaBar.SetValue(Mana);

        ManaChanged(amount);
    }

    protected virtual void ManaChanged(float delta) { }

}
