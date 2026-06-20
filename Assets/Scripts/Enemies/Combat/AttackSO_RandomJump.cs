using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomJump", menuName = "Enemy Data/Attacks/Random Jump (Hopper)")]
public class AttackSO_RandomJump : EnemyAttackSO
{
    [Header("Jump Physics")]
    public float hopForce = 6f;
    public float airTime = 0.5f;
    public float landingClearance = 0.4f;

    [Header("Landing Impact")]
    public float landingDamageRadius = 1.5f;
    public float landingDamage = 1f;

    [Header("Layer Setup")]
    public string defaultLayer = "GroundEnemy";
    public string flyingLayer = "FlyingEnemy";

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);

        float originalDamping = enemy.Rb.linearDamping;
        enemy.Rb.linearDamping = 0f;
        enemy.gameObject.layer = LayerMask.NameToLayer(flyingLayer);

        AStarGrid grid = enemy.GetComponentInParent<AStarGrid>();
        Vector2 dir = Random.insideUnitCircle.normalized;
        bool foundSafeDirection = false;

        if (grid != null)
        {
            float expectedDistance = hopForce * airTime;
            for (int i = 0; i < 20; i++)
            {
                Vector2 expectedLanding = (Vector2)enemy.transform.position + (dir * expectedDistance);
                var landingNode = grid.NodeFromWorldPoint(expectedLanding);
                bool hasEnoughSpace = Physics2D.OverlapCircle(expectedLanding, landingClearance, enemy.details.obstacleMask) == null;

                if (landingNode != null && landingNode.isWalkable && hasEnoughSpace)
                {
                    foundSafeDirection = true;
                    break;
                }
                dir = Random.insideUnitCircle.normalized;
            }
        }

        if (!foundSafeDirection) dir = Vector2.zero;

        enemy.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(airTime);

        enemy.Rb.linearDamping = originalDamping;
        enemy.Rb.linearVelocity = Vector2.zero;
        enemy.gameObject.layer = LayerMask.NameToLayer(defaultLayer);

        if (Physics2D.OverlapCircle(enemy.transform.position, 0.2f, enemy.details.obstacleMask) != null)
        {
            enemy.transform.position = FindNearestSafePosition(enemy.transform.position, enemy.details.obstacleMask);
        }

        if (enemy.Target != null && Vector2.Distance(enemy.transform.position, enemy.Target.position) <= landingDamageRadius)
        {
            if (enemy.Target.TryGetComponent(out IDamageable hit)) hit.TakeDamage(landingDamage);
        }

        yield return new WaitForSeconds(0.3f);
    }

    protected Vector2 FindNearestSafePosition(Vector2 startPos, LayerMask obstacleMask)
    {
        if (Physics2D.OverlapCircle(startPos, 0.3f, obstacleMask) == null) return startPos;

        float searchRadius = 0.4f;
        for (int i = 0; i < 10; i++)
        {
            for (int angle = 0; angle < 360; angle += 45)
            {
                float rad = angle * Mathf.Deg2Rad;
                Vector2 potentialPos = startPos + (new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * searchRadius);

                if (Physics2D.OverlapCircle(potentialPos, 0.3f, obstacleMask) == null) return potentialPos;
            }
            searchRadius += 0.3f;
        }
        return startPos;
    }
}