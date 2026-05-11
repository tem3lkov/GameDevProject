using System.Collections;
using UnityEngine;

public class MonstroJumpAttack : EnemyAttack {
    public float hopForce = 6f;
    public float airTime = 0.5f;

    public override IEnumerator Execute(Enemy enemy) {
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.SetTrigger("Prep");
        yield return new WaitForSeconds(0.2f);

        if (enemy.Target != null) {
            if (enemy.Anim != null) enemy.Anim.SetTrigger("Jump");

            float originalDamping = enemy.Rb.linearDamping;
            enemy.Rb.linearDamping = 0f;

            Vector2 dir = (enemy.Target.position - transform.position).normalized;
            enemy.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(airTime);

            enemy.Rb.linearDamping = originalDamping;
        } else {
            yield return new WaitForSeconds(airTime);
        }

        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.SetTrigger("Land");
        yield return new WaitForSeconds(0.2f);
        if (enemy.Anim != null) enemy.Anim.SetTrigger("Idle");
    }
}