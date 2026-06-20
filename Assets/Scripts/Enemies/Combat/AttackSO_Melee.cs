using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Enemy Data/Attacks/Melee")]
public class AttackSO_Melee : EnemyAttackSO
{
    public float hitRadius = 1.8f;
    public float damage = 1f;
    public float windupTime = 0.2f;
    public float recoveryTime = 0.4f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(windupTime);

        if (enemy.Target != null && Vector2.Distance(enemy.transform.position, enemy.Target.position) <= hitRadius)
        {
            if (enemy.Target.TryGetComponent(out IDamageable hit)) hit.TakeDamage(damage);
        }

        yield return new WaitForSeconds(recoveryTime);
    }
}