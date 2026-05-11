using System.Collections;
using UnityEngine;

public class MonstroJumpAttack : EnemyAttack {
    public float hopForce = 6f;
    public float airTime = 0.5f;

    public override IEnumerator Execute(Enemy user) {
        user.CanMove = false;
        user.Rb.linearVelocity = Vector2.zero;

        if (user.Anim != null) user.Anim.SetTrigger("Prep");
        yield return new WaitForSeconds(0.2f);

        if (user.Target != null) {
            if (user.Anim != null) user.Anim.SetTrigger("Jump");

            float originalDamping = user.Rb.linearDamping;
            user.Rb.linearDamping = 0f;

            Vector2 dir = (user.Target.position - transform.position).normalized;
            user.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(airTime);

            user.Rb.linearDamping = originalDamping;
        } else {
            yield return new WaitForSeconds(airTime);
        }

        user.Rb.linearVelocity = Vector2.zero;

        if (user.Anim != null) user.Anim.SetTrigger("Land");
        yield return new WaitForSeconds(0.2f);
        if (user.Anim != null) user.Anim.SetTrigger("Idle");

        user.CanMove = true;
    }
}