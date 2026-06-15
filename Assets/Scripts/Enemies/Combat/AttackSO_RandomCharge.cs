using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomCharge", menuName = "Enemy Data/Attacks/Random Charge (Dip)")]
public class AttackSO_RandomCharge : EnemyAttackSO
{
    [Header("Charge Settings")]
    [Tooltip("The force with which it is fired")]
    public float chargeForce = 5f;
    [Tooltip("How long does the sliding itself last?")]
    public float chargeDuration = 0.2f;
    [Tooltip("Waiting time between two speed ups")]
    public float waitTime = 1f;

    [Header("Visual Colors (Optional)")]
    public Color chargeColor = new Color(0.8f, 0.4f, 0.2f, 1f);

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");

        yield return new WaitForSeconds(waitTime);

        Vector2 dir;

        if (enemy.Target != null && Random.value > 0.5f)
        {
            dir = (enemy.Target.position - enemy.transform.position).normalized;
        } else
        {
            dir = Random.insideUnitCircle.normalized;
        }

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Move"); 
        if (enemy.SpriteRend != null) enemy.SpriteRend.color = chargeColor;

        float originalDamping = enemy.Rb.linearDamping;
        enemy.Rb.linearDamping = 2f;

        enemy.Rb.AddForce(dir * chargeForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(chargeDuration);

        enemy.Rb.linearDamping = originalDamping;
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.SpriteRend != null) enemy.SpriteRend.color = Color.white;
    }
}