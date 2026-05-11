using System;
using UnityEngine;

public enum EnemyState { Idle, Aggro, Dead }

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour, IDamageable {
    public EnemyStatsSO stats;

    public event Action<Enemy> OnDeath;
    public event Action<float> OnHealthChanged;

    public EnemyState CurrentState { get; private set; } = EnemyState.Idle;
    public Transform Target { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }
    public SpriteRenderer spriteRenderer;

    // Control flags
    public bool IsAttacking { get; set; } = false;
    protected float currentHealth;
    private float lastStateCheckTime;

    protected virtual void Awake() {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHealth = stats.maxHealth;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable hitTarget)) {
                hitTarget.TakeDamage(stats.damage);
            }
        }
    }

    protected virtual void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) Target = player.transform;
    }

    protected virtual void Update() {
        if (CurrentState == EnemyState.Dead || Target == null) return;

        if (Time.time >= lastStateCheckTime + 0.2f) {
            UpdateState();
            lastStateCheckTime = Time.time;
        }

        if (CurrentState == EnemyState.Aggro) {
            HandleAggroBehavior();
        }
    }

    private void UpdateState() {
        CurrentState = HasLineOfSight() ? EnemyState.Aggro : EnemyState.Idle;
    }

    private bool HasLineOfSight() {
        if (Target == null) return false;
        Vector2 direction = Target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, stats.obstacleMask);
        return hit.collider == null;
    }

    protected virtual void HandleAggroBehavior() { }

    public virtual void TakeDamage(float amount) {
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth / stats.maxHealth);
        if (currentHealth <= 0) Die();
    }

    public virtual void Die() {
        if (CurrentState == EnemyState.Dead) return;
        CurrentState = EnemyState.Dead;

        StopAllCoroutines();
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}