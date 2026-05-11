using System.Collections;
using UnityEngine;

public class JumpAttack : EnemyAttack {
    [Header("Hop Settings")]
    public float hopForce = 4f;
    public float airTime = 0.6f;

    [Header("Damage Settings")]
    [Tooltip("How much damage the player takes if they get landed on.")]
    public float landingDamage = 1f;
    [Tooltip("How close the enemy needs to land to the player to hurt them.")]
    public float landingDamageRadius = 1.2f;

    public override IEnumerator Execute(Enemy enemy) {
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.SetTrigger("Prep");
        yield return new WaitForSeconds(0.2f);

        if (enemy.Target != null) {
            if (enemy.Anim != null) enemy.Anim.SetTrigger("Jump");

            Vector2 dir = (enemy.Target.position - transform.position).normalized;
            enemy.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(airTime);

            enemy.Rb.linearVelocity = Vector2.zero;

            if (enemy.Target != null) {
                float distanceToPlayer = Vector2.Distance(transform.position, enemy.Target.position);

                if (distanceToPlayer <= landingDamageRadius) {
                    if (enemy.Target.TryGetComponent<IDamageable>(out IDamageable hitTarget)) {
                        hitTarget.TakeDamage(landingDamage);
                    }
                }
            }
        }

        if (enemy.Anim != null) enemy.Anim.SetTrigger("Land");
        yield return new WaitForSeconds(0.2f);
    }
}