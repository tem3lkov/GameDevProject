using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class MeleeAttack : MonoBehaviour {
    [Header("Melee Settings")]
    public float attackRange = 1.5f;
    public float damage = 1f; 

    private Enemy enemy;
    private float nextAttackTime;

    private void Awake() {
        enemy = GetComponent<Enemy>();
    }

    private void Update() {
        if (enemy.CurrentState == EnemyState.Aggro && enemy.Target != null) {

            float distanceToTarget = Vector2.Distance(transform.position, enemy.Target.position);

            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime) {
                AttackTarget();

                nextAttackTime = Time.time + enemy.stats.attackCooldown;
            }
        }
    }

    private void AttackTarget() {
        if (enemy.Target.TryGetComponent<IDamageable>(out IDamageable hitTarget)) {
            hitTarget.TakeDamage(damage);

        }
    }
}