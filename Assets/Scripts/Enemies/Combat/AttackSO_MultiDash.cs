using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiDashAttack", menuName = "Enemy Data/Attacks/Multi Dash")]
public class AttackSO_MultiDash : EnemyAttackSO
{
    public int numberOfDashes = 4;
    public float dashSpeed = 6f;
    public float dashDuration = 0.80f;
    public float timeBetweenDashes = 0.15f;
    public float chargeUpTime = 0.3f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        AStarGrid grid = enemy.GetComponentInParent<AStarGrid>();

        for (int i = 0; i < numberOfDashes; i++)
        {
            enemy.Rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(chargeUpTime);

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
            }

            yield return new WaitForSeconds(dashDuration);

            enemy.Rb.linearVelocity = Vector2.zero;
            if (i < numberOfDashes - 1) yield return new WaitForSeconds(timeBetweenDashes);
        }
    }
}