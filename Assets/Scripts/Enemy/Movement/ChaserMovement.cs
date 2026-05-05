using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ChaserMovement : MonoBehaviour {
    private Enemy enemy;

    private void Awake() {
        enemy = GetComponent<Enemy>();
    }

    private void Update() {
        if (enemy.CurrentState == EnemyState.Aggro && enemy.Target != null) {
            Vector2 direction = (enemy.Target.position - transform.position).normalized;
            enemy.Rb.linearVelocity = direction * enemy.stats.movementSpeed;
        } else if (enemy.CurrentState == EnemyState.Idle) {
            enemy.Rb.linearVelocity = Vector2.zero;
        }
    }
}