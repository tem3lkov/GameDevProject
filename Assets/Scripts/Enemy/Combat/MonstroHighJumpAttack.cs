using System.Collections;
using UnityEngine;

public class MonstroHighJumpAttack : EnemyAttack {
    [Header("Jump Settings")]
    public float trackingTime = 1.5f;
    public float lockInTime = 0.6f;
    public GameObject projectilePrefab;

    [Header("Damage & Impact")]
    public float landingDamage = 1f;
    public float landingDamageRadius = 1.5f;
    public float bumpKnockbackForce = 15f;

    public override IEnumerator Execute(Enemy user) {
        if (user == null) yield break;

        user.Rb.linearVelocity = Vector2.zero;
        Vector3 originalScale = transform.localScale;
        Collider2D myCollider = user.GetComponent<Collider2D>();

        if (user.Anim != null) user.Anim.SetTrigger("Prep");
        yield return new WaitForSeconds(0.4f);

        if (myCollider != null) myCollider.enabled = false;
        if (user.Anim != null) user.Anim.SetTrigger("Jump");

        float originalDamping = user.Rb.linearDamping;
        user.Rb.linearDamping = 0f;
        user.Rb.linearVelocity = Vector2.up * 15f;

        float elapsed = 0;
        float shrinkDuration = 0.3f;
        while (elapsed < shrinkDuration) {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.1f, elapsed / shrinkDuration);
            yield return null;
        }

        user.Rb.linearVelocity = Vector2.zero;

        float trackingElapsed = 0;
        while (trackingElapsed < trackingTime) {
            trackingElapsed += Time.deltaTime;
            if (user.Target != null) {
                Vector2 playerPos = user.Target.position;
                Vector2 desiredPos = playerPos + new Vector2(0, 1.5f);

                RaycastHit2D hit = Physics2D.Linecast(playerPos, desiredPos, user.stats.obstacleMask);

                if (hit.collider != null) {
                    desiredPos = hit.point + (hit.normal * 0.8f);
                }

                user.Rb.MovePosition(desiredPos);
            }
            yield return null;
        }

        yield return new WaitForSeconds(lockInTime);

        if (user.Anim != null) user.Anim.SetTrigger("Land");

        elapsed = 0;
        float growDuration = 0.15f;
        while (elapsed < growDuration) {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale * 0.1f, originalScale, elapsed / growDuration);
            yield return null;
        }
        transform.localScale = originalScale;

        user.Rb.linearDamping = originalDamping;
        user.Rb.linearVelocity = Vector2.zero;

        if (myCollider != null) myCollider.enabled = true;

        HandleLandingImpact(user);

        SpawnRadialBurst(user);

        yield return new WaitForSeconds(0.5f);
        if (user.Anim != null) user.Anim.SetTrigger("Idle");
    }

    private void HandleLandingImpact(Enemy user) {
        if (user.Target == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, user.Target.position);
        if (distanceToPlayer <= landingDamageRadius) {
            if (user.Target.TryGetComponent<IDamageable>(out IDamageable hitTarget)) {
                hitTarget.TakeDamage(landingDamage);
            }

            if (user.Target.TryGetComponent<Rigidbody2D>(out Rigidbody2D playerRb)) {
                Vector2 knockbackDir = (user.Target.position - transform.position).normalized;
                if (knockbackDir == Vector2.zero) knockbackDir = Random.insideUnitCircle.normalized;
                playerRb.AddForce(knockbackDir * bumpKnockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    private void SpawnRadialBurst(Enemy enemy) {
        if (projectilePrefab == null) return;

        for (int i = 0; i < 8; i++) {
            float angle = i * 45f;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;

            Vector3 spawnPos = transform.position + (Vector3)(dir * 0.2f);

            GameObject tear = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            if (tear.TryGetComponent<Rigidbody2D>(out Rigidbody2D tearRb)) {
                tearRb.linearVelocity = dir * 6f;
            }
        }
    }
}