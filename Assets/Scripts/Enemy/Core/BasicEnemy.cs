using System.Collections;
using UnityEngine;

public class BasicEnemy : Enemy {
    [Header("Combat Settings")]
    public EnemyAttack[] allAttacks;

    private float nextAttackTime;

    protected virtual void FixedUpdate() {
        if (!IsAttacking) {
            Rb.linearVelocity = Vector2.MoveTowards(Rb.linearVelocity, Vector2.zero, 15f * Time.deltaTime);
        }
    }

    protected override void HandleAggroBehavior() {
        if (IsAttacking || Time.time < nextAttackTime) return;

        foreach (EnemyAttack attack in allAttacks) {
            if (attack.CanExecute(this)) {
                StartCoroutine(AttackRoutine(attack));
                break;
            }
        }
    }

    private IEnumerator AttackRoutine(EnemyAttack chosenAttack) {
        IsAttacking = true;
        yield return StartCoroutine(chosenAttack.Execute(this));
        IsAttacking = false;
        nextAttackTime = Time.time + stats.attackCooldown;
    }
}