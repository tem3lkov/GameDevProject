using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MonstroHighJumpAttack", menuName = "Enemy Data/Attacks/Monstro High Jump")]
public class AttackSO_MonstroHighJump : EnemyAttackSO
{
    [Header("Jump Settings")]
    public float trackingTime = 1.5f;
    public float lockInTime = 0.6f;
    public GameObject projectilePrefab;
    public LayerMask obstacleMask;

    [Header("Damage & Impact")]
    public float landingDamage = 1f;
    public float landingDamageRadius = 1.5f;
    public float bumpKnockbackForce = 15f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        if (enemy == null) yield break;

        enemy.Rb.linearVelocity = Vector2.zero;
        Vector3 originalScale = enemy.transform.localScale;

        Collider2D[] allColliders = enemy.GetComponents<Collider2D>();

        // 1. Windup
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Prep");
        yield return new WaitForSeconds(0.4f);

        foreach (Collider2D col in allColliders) col.enabled = false;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Jump");

        float originalDamping = enemy.Rb.linearDamping;
        enemy.Rb.linearDamping = 0f;
        enemy.Rb.linearVelocity = Vector2.up * 15f;

        float elapsed = 0;
        float shrinkDuration = 0.3f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            enemy.transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.1f, elapsed / shrinkDuration);
            yield return null;
        }

        enemy.Rb.linearVelocity = Vector2.zero;

        float trackingElapsed = 0;
        float monstroRadius = 0.6f;

        while (trackingElapsed < trackingTime)
        {
            trackingElapsed += Time.deltaTime;
            if (enemy.Target != null)
            {
                Vector2 desiredPos = enemy.Target.position;

                Collider2D wallHit = Physics2D.OverlapCircle(desiredPos, monstroRadius, obstacleMask);

                if (wallHit != null)
                {
                    Vector2 closestPointOnWall = wallHit.ClosestPoint(desiredPos);

                    Vector2 pushAwayDirection = (desiredPos - closestPointOnWall).normalized;

                    if (pushAwayDirection == Vector2.zero) pushAwayDirection = Vector2.down;

                    desiredPos = closestPointOnWall + (pushAwayDirection * monstroRadius);
                }

                enemy.Rb.MovePosition(desiredPos);
            }
            yield return null;
        }

        yield return new WaitForSeconds(lockInTime);

        // 4. Fall Down
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Land");

        elapsed = 0;
        float growDuration = 0.15f;
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            enemy.transform.localScale = Vector3.Lerp(originalScale * 0.1f, originalScale, elapsed / growDuration);
            yield return null;
        }

        enemy.transform.localScale = originalScale;
        enemy.Rb.linearDamping = originalDamping;
        enemy.Rb.linearVelocity = Vector2.zero;

        foreach (Collider2D col in allColliders) col.enabled = true;

        HandleLandingImpact(enemy);
        SpawnRadialBurst(enemy);

        yield return new WaitForSeconds(0.5f);
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
    }

    private void HandleLandingImpact(EnemyController enemy)
    {
        if (enemy.Target == null) return;

        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.Target.position);
        if (distanceToPlayer <= landingDamageRadius)
        {
            if (enemy.Target.TryGetComponent(out IDamageable hitTarget)) hitTarget.TakeDamage(landingDamage);

            if (enemy.Target.TryGetComponent(out Rigidbody2D playerRb))
            {
                Vector2 knockbackDir = (enemy.Target.position - enemy.transform.position).normalized;
                if (knockbackDir == Vector2.zero) knockbackDir = Random.insideUnitCircle.normalized;
                playerRb.AddForce(knockbackDir * bumpKnockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    private void SpawnRadialBurst(EnemyController enemy)
    {
        if (projectilePrefab == null) return;

        for (int i = 0; i < 8; i++)
        {
            Vector2 dir = Quaternion.Euler(0, 0, i * 45f) * Vector2.up;
            Vector3 spawnPos = enemy.transform.position + (Vector3)(dir * 0.2f);

            GameObject tear = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            if (tear.TryGetComponent(out Rigidbody2D tearRb)) tearRb.linearVelocity = dir * 6f;
        }
    }
}