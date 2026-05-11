using System.Collections;
using UnityEngine;

public class Attack_Melee : EnemyAttack {
    [Header("Melee Settings")]
    [Tooltip("The actual area of the swing. Do not confuse with the base maxRange which tells the AI when to start the attack.")]
    public float hitRadius = 1.8f;
    public float damage = 1f;

    public override IEnumerator Execute(Enemy user) {
        user.CanMove = false;
        user.Rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f);

        if (user.Target != null) {
            float distanceToTarget = Vector2.Distance(transform.position, user.Target.position);

            if (distanceToTarget <= hitRadius) {
                if (user.Target.TryGetComponent<IDamageable>(out IDamageable hitTarget)) {
                    hitTarget.TakeDamage(damage);
                }
            }
        }

        yield return new WaitForSeconds(0.4f);

        user.CanMove = true;
    }
}