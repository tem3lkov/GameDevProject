using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundWormAttack", menuName = "Enemy Data/Attacks/Round Worm")]
public class AttackSO_RoundWorm : EnemyAttackSO
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 6f;
    public float damage = 1f;

    [Header("Worm Timings")]
    public float popUpDuration = 0.5f;
    public float aimDuration = 0.3f;
    public float hideDuration = 0.5f;

    [Header("Teleport Settings")]
    public bool teleportUnderground = true;
    public float teleportRadiusFromPlayer = 5f;
    public float spawnClearanceRadius = 0.5f;
    public int maxSpawnAttempts = 10;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;

        if (teleportUnderground && enemy.Target != null)
        {
            enemy.transform.position = FindSafeSpawnPosition(enemy);
        }

        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
        if (enemy.SpriteRend != null) enemy.SpriteRend.enabled = true;

        yield return new WaitForSeconds(popUpDuration + aimDuration);

        if (projectilePrefab != null && enemy.Target != null)
        {
            Vector2 direction = (enemy.Target.position - enemy.transform.position).normalized;
            float checkDistance = Mathf.Max(0.1f, Vector2.Distance(enemy.Target.position, enemy.transform.position) - 0.5f);

            RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, direction, checkDistance, enemy.details.obstacleMask);

            if (hit.collider == null)
            {
                GameObject tear = Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
                if (tear.TryGetComponent(out Projectile proj)) proj.damage = damage;
                if (tear.TryGetComponent(out Rigidbody2D projRb)) projRb.linearVelocity = direction * projectileSpeed;
            }
        }

        yield return new WaitForSeconds(hideDuration);

        if (col != null) col.enabled = false;
        if (enemy.SpriteRend != null) enemy.SpriteRend.enabled = false;
    }

    private Vector2 FindSafeSpawnPosition(EnemyController enemy)
    {
        LayerMask obstacleMask = enemy.details.obstacleMask;
        Collider2D roomBounds = enemy.GetComponentInParent<RoomEncounter>()?.GetComponent<Collider2D>();

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * teleportRadiusFromPlayer;
            Vector2 potentialPosition = (Vector2)enemy.Target.position + randomOffset;

            if (roomBounds != null && !roomBounds.bounds.Contains(potentialPosition)) continue;

            Collider2D pointHit = Physics2D.OverlapCircle(potentialPosition, spawnClearanceRadius, obstacleMask);

            Vector2 dirFromPlayer = (potentialPosition - (Vector2)enemy.Target.position).normalized;
            float distFromPlayer = Vector2.Distance(enemy.Target.position, potentialPosition);
            RaycastHit2D wallHit = Physics2D.Raycast(enemy.Target.position, dirFromPlayer, distFromPlayer, obstacleMask);

            if (pointHit == null && wallHit.collider == null)
            {
                return potentialPosition;
            }
        }
        return enemy.transform.position;
    }
}