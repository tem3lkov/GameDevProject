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
            Vector2 safePosition = FindSafeSpawnPosition(enemy);
            enemy.transform.position = safePosition;
        }

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("PopUp");

        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        yield return new WaitForSeconds(popUpDuration);

        // 2. Прицелване и проверка за видимост (Line of Sight)
        if (projectilePrefab != null && enemy.Target != null)
        {
            if (enemy.Anim != null) enemy.Anim.PlayAnimation("Shoot");
            yield return new WaitForSeconds(aimDuration);

            Vector2 direction = (enemy.Target.position - enemy.transform.position).normalized;
            float fullDistance = Vector2.Distance(enemy.Target.position, enemy.transform.position);

            float checkDistance = Mathf.Max(0.1f, fullDistance - 0.5f);

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(enemy.details.obstacleMask);
            filter.useTriggers = false;

            RaycastHit2D[] results = new RaycastHit2D[1];
            int hitCount = Physics2D.Raycast(enemy.transform.position, direction, filter, results, checkDistance);

            if (hitCount == 0 || results[0].collider.gameObject == enemy.gameObject)
            {
                GameObject tear = Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
                if (tear.TryGetComponent(out Rigidbody2D projRb))
                {
                    projRb.linearVelocity = direction * projectileSpeed;
                }
            } else
            {
                Debug.Log($"Shot blocked by: {results[0].collider.gameObject.name}");
            }
        } else
        {
            yield return new WaitForSeconds(aimDuration);
        }

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Hide");
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

            if (roomBounds != null && !roomBounds.bounds.Contains(potentialPosition))
            {
                continue;
            }

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