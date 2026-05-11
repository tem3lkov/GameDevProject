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

    public override IEnumerator Execute(Enemy user) {
        user.CanMove = false;
        user.Rb.linearVelocity = Vector2.zero;

        if (user.Anim != null) user.Anim.SetTrigger("Prep");
        yield return new WaitForSeconds(0.2f);

        if (user.Target != null) {
            if (user.Anim != null) user.Anim.SetTrigger("Jump");

            Vector2 dir = (user.Target.position - transform.position).normalized;
            user.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(airTime);

            user.Rb.linearVelocity = Vector2.zero;

            if (user.Target != null) {
                float distanceToPlayer = Vector2.Distance(transform.position, user.Target.position);

                if (distanceToPlayer <= landingDamageRadius) {
                    if (user.Target.TryGetComponent<IDamageable>(out IDamageable hitTarget)) {
                        hitTarget.TakeDamage(landingDamage);
                    }
                }
            }
        }

        if (user.Anim != null) user.Anim.SetTrigger("Land");
        yield return new WaitForSeconds(0.2f);

        user.CanMove = true;
    }
}