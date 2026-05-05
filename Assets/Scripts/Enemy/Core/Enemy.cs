using System;
using UnityEngine;

public enum EnemyState { Idle, Aggro, Dead }

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour, IDamageable {
    public EnemyStatsSO stats;
    public event Action<Enemy> OnDeath;

    public EnemyState CurrentState { get; private set; } = EnemyState.Idle;
    public Transform Target { get; private set; }

    public Rigidbody2D Rb { get; private set; }

    private float currentHealth;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake() {
        Rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHealth = stats.maxHealth;
    }

    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) Target = player.transform;
    }

    private void Update() {
        if (CurrentState == EnemyState.Dead || Target == null) return;
        UpdateState();
    }

    private void UpdateState() {
        if (HasLineOfSight()) {
            CurrentState = EnemyState.Aggro;
        } else {
            CurrentState = EnemyState.Idle;
        }
    }

    private bool HasLineOfSight() {
        if (Target == null) return false;
        Vector2 direction = Target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, stats.obstacleMask);
        return hit.collider == null;
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    public void Die() {
        if (CurrentState == EnemyState.Dead) return;
        CurrentState = EnemyState.Dead;

        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}