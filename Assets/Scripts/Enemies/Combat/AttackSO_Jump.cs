using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalJump", menuName = "Enemy Data/Attacks/Normal Jump")]
public class AttackSO_Jump : EnemyAttackSO
{
    public float hopForce = 6f;
    public float airTime = 0.5f;
    public float landingDamageRadius = 1.5f;
    public float landingDamage = 1f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;

        Collider2D myCollider = enemy.GetComponent<Collider2D>();

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Prep");
        yield return new WaitForSeconds(0.2f);

        if (enemy.Target != null)
        {
            if (enemy.Anim != null) enemy.Anim.PlayAnimation("Jump");

            float originalDamping = enemy.Rb.linearDamping;
            enemy.Rb.linearDamping = 0f;

            if (myCollider != null) myCollider.enabled = false;

            Vector2 dir = (enemy.Target.position - enemy.transform.position).normalized;
            enemy.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(airTime);

            enemy.Rb.linearDamping = originalDamping;
        }

        enemy.Rb.linearVelocity = Vector2.zero;

        if (myCollider != null) myCollider.enabled = true;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Land");

        if (enemy.Target != null)
        {
            float distance = Vector2.Distance(enemy.transform.position, enemy.Target.position);
            if (distance <= landingDamageRadius)
            {
                if (enemy.Target.TryGetComponent(out IDamageable hit)) hit.TakeDamage(landingDamage);
            }
        }

        yield return new WaitForSeconds(0.3f);
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
    }
}