using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomCharge", menuName = "Enemy Data/Attacks/Random Charge (Dip)")]
public class AttackSO_RandomCharge : EnemyAttackSO
{
    [Header("Charge Settings")]
    public float chargeForce = 5f;
    public float chargeDuration = 0.2f;
    public float waitTime = 1f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);

        Vector2 dir = (enemy.Target != null && Random.value > 0.5f)
            ? (Vector2)(enemy.Target.position - enemy.transform.position).normalized
            : Random.insideUnitCircle.normalized;

        float originalDamping = enemy.Rb.linearDamping;
        enemy.Rb.linearDamping = 2f;
        enemy.Rb.AddForce(dir * chargeForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(chargeDuration);

        enemy.Rb.linearDamping = originalDamping;
        enemy.Rb.linearVelocity = Vector2.zero;
    }
}