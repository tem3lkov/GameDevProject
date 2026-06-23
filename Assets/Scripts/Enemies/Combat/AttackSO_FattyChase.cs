using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FattyChase", menuName = "Enemy Data/Attacks/Fatty Chase")]
public class AttackSO_FattyChase : EnemyAttackSO
{
    [Header("Chase Settings")]
    public float chaseSpeed = 1.5f;
    public float chaseDuration = 3f;
    public float restDuration = 0.5f;
    public float repathRate = 0.2f;
    public float nextWaypointDistance = 0.4f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        float timer = 0f;
        float repathTimer = 0f;
        List<Vector2> currentPath = null;
        int currentWaypointIndex = 0;

        while (timer < chaseDuration)
        {
            if (enemy.Target != null)
            {
                repathTimer -= Time.deltaTime;

                // Use the cached values directly!
                Vector2 feetPos = (Vector2)enemy.transform.position + enemy.FeetOffset;

                if (repathTimer <= 0f)
                {
                    if (enemy.RoomGrid != null)
                    {
                        currentPath = AStarPathfinder.FindPath(enemy.RoomGrid, feetPos, (Vector2)enemy.Target.position);
                        currentWaypointIndex = 0;
                    }
                    repathTimer = repathRate;
                }

                Vector2 moveDirection = ((Vector2)enemy.Target.position - feetPos).normalized;

                if (currentPath != null && currentWaypointIndex < currentPath.Count)
                {
                    Vector2 targetWaypoint = currentPath[currentWaypointIndex];
                    moveDirection = (targetWaypoint - feetPos).normalized;

                    if (Vector2.Distance(feetPos, targetWaypoint) < nextWaypointDistance)
                    {
                        currentWaypointIndex++;
                    }
                }

                enemy.Rb.linearVelocity = moveDirection * chaseSpeed;
            } else
            {
                enemy.Rb.linearVelocity = Vector2.zero;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        enemy.Rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(restDuration);
    }
}