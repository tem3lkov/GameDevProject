using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Enemy Data/Attacks/Melee")]
public class AttackSO_Melee : EnemyAttackSO
{
    public float hitRadius = 1.8f;
    public float damage = 1f;
    private bool actionFired = false;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        actionFired = false;
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null)
        {
            enemy.Anim.OnAnimationActionTriggered += DoMelee;
            enemy.Anim.PlayAnimation("Attack");

            while (!actionFired) yield return null;

            enemy.Anim.OnAnimationActionTriggered -= DoMelee;
        } else
        {
            yield return new WaitForSeconds(0.2f);
            DoMelee();
        }

        yield return new WaitForSeconds(0.4f);
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");

        void DoMelee()
        {
            actionFired = true;
            if (enemy.Target != null && Vector2.Distance(enemy.transform.position, enemy.Target.position) <= hitRadius)
            {
                if (enemy.Target.TryGetComponent(out IDamageable hit)) hit.TakeDamage(damage);
            }
        }
    }
}