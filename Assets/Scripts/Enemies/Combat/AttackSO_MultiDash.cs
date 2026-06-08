using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiDashAttack", menuName = "Enemy Data/Attacks/Multi Dash")]
public class AttackSO_MultiDash : EnemyAttackSO
{
    [Header("Dash Sequence Settings")]
    public int numberOfDashes = 4;
    public float dashSpeed = 6f;
    public float dashDuration = 0.80f;
    [Tooltip("How long it pauses between each dash in the chain")]
    public float timeBetweenDashes = 0.15f;

    [Header("Animation Triggers")]
    public string chargeAnimTrigger = "Charge";
    public string dashAnimTrigger = "Dash";

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        AStarGrid grid = enemy.GetComponentInParent<AStarGrid>();

        for (int i = 0; i < numberOfDashes; i++)
        {
            enemy.Rb.linearVelocity = Vector2.zero;
            bool readyToDash = false;

            void LaunchDash() { readyToDash = true; }

            if (enemy.Anim != null)
            {
                enemy.Anim.OnAnimationActionTriggered += LaunchDash;
                enemy.Anim.PlayAnimation(chargeAnimTrigger);

                while (!readyToDash) yield return null;

                enemy.Anim.OnAnimationActionTriggered -= LaunchDash;
            }

            if (enemy.Target != null)
            {
                Vector2 dashDirection = (enemy.Target.position - enemy.transform.position).normalized;

                if (grid != null)
                {
                    List<Vector2> path = AStarPathfinder.FindPath(grid, enemy.transform.position, enemy.Target.position);

                    if (path != null && path.Count > 0)
                    {
                        int lookAheadIndex = Mathf.Min(2, path.Count - 1);
                        dashDirection = (path[lookAheadIndex] - (Vector2)enemy.transform.position).normalized;
                    }
                }

                enemy.Rb.linearVelocity = dashDirection * dashSpeed;

                if (enemy.Anim != null) enemy.Anim.PlayAnimation(dashAnimTrigger);
            }

            yield return new WaitForSeconds(dashDuration);

            enemy.Rb.linearVelocity = Vector2.zero;

            if (i < numberOfDashes - 1)
            {
                if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
                yield return new WaitForSeconds(timeBetweenDashes);
            }
        }

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
    }
}