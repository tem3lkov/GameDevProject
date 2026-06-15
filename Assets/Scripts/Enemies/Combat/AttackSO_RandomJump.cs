using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomJump", menuName = "Enemy Data/Attacks/Random Jump (Hopper)")]
public class AttackSO_RandomJump : EnemyAttackSO
{
    [Header("Jump Physics")]
    public float hopForce = 6f;
    public float airTime = 0.5f;

    [Tooltip("How wide should the landing spot be")]
    public float landingClearance = 0.4f;

    [Header("Landing Impact")]
    public float landingDamageRadius = 1.5f;
    public float landingDamage = 1f;

    [Header("Layer Setup")]
    public string defaultLayer = "GroundEnemy";
    public string flyingLayer = "FlyingEnemy";

    [Header("Visual Colors")]
    public Color prepColor = Color.red;
    public Color airColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Prep");
        if (enemy.SpriteRend != null) enemy.SpriteRend.color = prepColor;

        yield return new WaitForSeconds(0.2f);

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Jump");
        if (enemy.SpriteRend != null) enemy.SpriteRend.color = airColor;

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

        if (!foundSafeDirection)
        {
            dir = Vector2.zero;
        }

        enemy.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(airTime);

        enemy.Rb.linearDamping = originalDamping;
        enemy.Rb.linearVelocity = Vector2.zero;

        enemy.gameObject.layer = LayerMask.NameToLayer(defaultLayer);

        if (Physics2D.OverlapCircle(enemy.transform.position, 0.2f, enemy.details.obstacleMask) != null)
        {
            enemy.transform.position = FindNearestSafePosition(enemy.transform.position, enemy.details.obstacleMask);
        }

        if (enemy.SpriteRend != null) enemy.SpriteRend.color = Color.white;
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

    protected Vector2 FindNearestSafePosition(Vector2 startPos, LayerMask obstacleMask)
    {
        if (Physics2D.OverlapCircle(startPos, 0.3f, obstacleMask) == null)
            return startPos;

        float searchRadius = 0.4f;
        for (int i = 0; i < 10; i++)
        {
            for (int angle = 0; angle < 360; angle += 45)
            {
                float rad = angle * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * searchRadius;
                Vector2 potentialPos = startPos + offset;

                if (Physics2D.OverlapCircle(potentialPos, 0.3f, obstacleMask) == null)
                {
                    return potentialPos;
                }
            }
            searchRadius += 0.3f;
        }
        return startPos;
    }
}