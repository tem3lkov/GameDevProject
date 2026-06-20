using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MonstroJumpAttack", menuName = "Enemy Data/Attacks/Monstro Jump")]
public class AttackSO_MonstroJump : EnemyAttackSO
{
    public float hopForce = 6f;
    public float airTime = 0.5f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Prep");
        yield return new WaitForSeconds(0.2f);

        if (enemy.Target != null)
        {
            if (enemy.Anim != null) enemy.Anim.PlayAnimation("Jump");

            float originalDamping = enemy.Rb.linearDamping;
            enemy.Rb.linearDamping = 0f;

            Vector2 dir = (enemy.Target.position - enemy.transform.position).normalized;
            enemy.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(airTime);
            enemy.Rb.linearDamping = originalDamping;
        } else
        {
            yield return new WaitForSeconds(airTime);
        }

        enemy.Rb.linearVelocity = Vector2.zero;
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Land");
        yield return new WaitForSeconds(0.2f);
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
    }
}