using UnityEngine;

public abstract class LivingEntity : MonoBehaviour {

    [SerializeField] private string _name;
    public string Name => _name;


    [SerializeField] protected float _maxHealth = 100f;
    public float MaxHealth => _maxHealth;
    public float Health { get; private set; }
    public float HealthRatio => _maxHealth <= 0 ? 0f : Health / _maxHealth;
    [SerializeField] protected float _flatArmor = 0f;
    [SerializeField] private bool invincible;
    [SerializeField] private BarUI bar;

    public abstract EntityType EntityType { get; }
    public bool IsPlayer => EntityType == EntityType.Player;
    public bool IsMonster => EntityType != EntityType.Player;

    private bool _dead = false;
    public bool IsDead => _dead || Health <= 0;

    private SpriteRenderer _renderer;
#if UNITY_EDITOR
    public SpriteRenderer SpriteRenderer => _renderer == null ? GetComponentInChildren<SpriteRenderer>() : _renderer; // DEBUG !!
#else
    public SpriteRenderer SpriteRenderer => _renderer;
#endif

    private Hurtbox _hurtbox;
#if UNITY_EDITOR
    public Hurtbox Hurtbox => _hurtbox == null ? GetComponentInChildren<Hurtbox>() : _hurtbox; // DEBUG !!
#else
    public Hurtbox Hurtbox => _hurtbox;
#endif

    private void Awake() {
        InitEntity();
	}

	protected bool Initialized { get; private set; }
    protected virtual void InitEntity() {
        if(Initialized)
            return;
        Initialized = true;
        SetFullHealth();
        if(!_renderer)
            _renderer = GetComponent<SpriteRenderer>();
        if(!_renderer)
            _renderer = GetComponentInChildren<SpriteRenderer>();
        if(!_renderer)
            Debug.LogWarning("Player do NOT have any renderer !");
        if(!_hurtbox)
            _hurtbox = GetComponentInChildren<Hurtbox>();
    }

    public void SetFullHealth() {
        Health = _maxHealth;
	}
    public float Damage(float damage) {
        // Damage reduction
        damage -= GetFlatArmor();
        if(damage < 0 || IsDead || invincible) {
            SpawnDamageText("[Blocked]", DamageText.DamageType.Blocked);
            return 0;
        }

        // Damage effect
        Health -= damage;
        if(Health <= 0) {
            Die();
        }

        HealthChanged(-damage);
        DisplayHealthDelta(-damage, true);

        return damage;
    }


    public void Heal(float heal, bool showText = true) {
        if(heal < 0 || IsDead)
            return;

        // Heal effect
        Health += heal;
        if(Health > MaxHealth)
            Health = MaxHealth;

        HealthChanged(heal);
        DisplayHealthDelta(heal, showText);
    }
    protected virtual float GetFlatArmor() {
        return _flatArmor;
    }

    protected virtual void HealthChanged(float delta) { }

    protected virtual void DisplayHealthDelta(float delta, bool show) {
        if(show) {
            SpawnDamageText((delta > 0 ? "+" : "") + (Mathf.Abs(delta) < 1f ? "0" : "") + delta.ToString("#.##"), DamageText.GetTypeFromEntityType(EntityType, delta < 0));
        }
        if(bar)
            bar.SetValue(Health);
        HealthChanged(delta);
    }

    protected void SpawnDamageText(string text, DamageText.DamageType type) {
        //TODO
	}

    public virtual void Die() {
        _dead = true;
        Destroy(gameObject);
    }

}
