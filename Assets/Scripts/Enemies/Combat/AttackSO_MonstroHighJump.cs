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
        Collider2D myCollider = enemy.GetComponent<Collider2D>();

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Prep");
        yield return new WaitForSeconds(0.4f);

        if (myCollider != null) myCollider.enabled = false;
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
        while (trackingElapsed < trackingTime) 
        {
            trackingElapsed += Time.deltaTime;
            if (enemy.Target != null) 
            {
                Vector2 playerPos = enemy.Target.position;
                Vector2 desiredPos = playerPos + new Vector2(0, 1.5f);

                RaycastHit2D hit = Physics2D.Linecast(playerPos, desiredPos, obstacleMask);
                if (hit.collider != null) 
                {
                    desiredPos = hit.point + (hit.normal * 0.8f);
                }

                enemy.Rb.MovePosition(desiredPos);
            }
            yield return null;
        }

        yield return new WaitForSeconds(lockInTime);

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
        if (myCollider != null) myCollider.enabled = true;

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
            if (enemy.Target.TryGetComponent<IDamageable>(out IDamageable hitTarget)) 
            {
                hitTarget.TakeDamage(landingDamage);
            }

            if (enemy.Target.TryGetComponent<Rigidbody2D>(out Rigidbody2D playerRb)) 
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
            float angle = i * 45f;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
            Vector3 spawnPos = enemy.transform.position + (Vector3)(dir * 0.2f);

            GameObject tear = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            if (tear.TryGetComponent<Rigidbody2D>(out Rigidbody2D tearRb)) 
            {
                tearRb.linearVelocity = dir * 6f;
            }
        }
    }
}