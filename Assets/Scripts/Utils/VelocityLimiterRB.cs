using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VelocityLimiterRB : MonoBehaviour {

    [SerializeField] private float _maxVelocity = 5f;
    private float _sqrMaxVelocity = 5f;
    private Rigidbody2D _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        SetMaxVelocity(_maxVelocity);
    }

    public void SetMaxVelocity(float maxVelocity) {
        this._maxVelocity = maxVelocity;
        this._sqrMaxVelocity = maxVelocity * maxVelocity;
    }

    private void  FixedUpdate() {
        var v = _rb.velocity;
        // We use squared values for performance issues
        if(v.sqrMagnitude > _sqrMaxVelocity) {
            _rb.velocity = v.normalized * _maxVelocity;
        }
    }
}