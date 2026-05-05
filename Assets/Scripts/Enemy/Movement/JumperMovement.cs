using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class JumperMovement : MonoBehaviour {
    private Enemy enemy;
    private bool isJumping = false;
    private float nextAttackTime;

    private void Awake() {
        enemy = GetComponent<Enemy>();
    }

    private void Update() {
        if (enemy.CurrentState == EnemyState.Aggro && !isJumping && enemy.Target != null) {
            if (Time.time >= nextAttackTime) {
                StartCoroutine(JumpRoutine());
            } else {
                enemy.Rb.linearVelocity = Vector2.zero;
            }
        } else if (enemy.CurrentState == EnemyState.Idle && !isJumping) {
            enemy.Rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator JumpRoutine() {
        isJumping = true;
        yield return new WaitForSeconds(0.2f); 

        if (enemy.Target != null) {
            Vector2 direction = (enemy.Target.position - transform.position).normalized;
            enemy.Rb.AddForce(direction * enemy.stats.movementSpeed, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.5f);
        enemy.Rb.linearVelocity = Vector2.zero;
        nextAttackTime = Time.time + enemy.stats.attackCooldown;
        isJumping = false;
    }
}