using System.Collections;
using UnityEngine;

public class Attack_RadialShoot : EnemyAttack {
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public int projectileCount = 8;
    public float projectileSpeed = 6f;
    public float damage = 1f;

    [Header("Spawn Settings")]
    [Tooltip("How far from the center should projectiles spawn to clear the boss's collider?")]
    public float spawnDistance = 1.5f;

    public override IEnumerator Execute(Enemy enemy) {
        enemy.Rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        if (projectilePrefab != null) {
            float angleStep = 360f / projectileCount;
            for (int i = 0; i < projectileCount; i++) {
                float angle = i * angleStep;
                Vector2 shotDir = Quaternion.Euler(0, 0, angle) * Vector2.up;

                Vector3 spawnPos = transform.position + (Vector3)(shotDir * spawnDistance);

                GameObject tear = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                if (tear.TryGetComponent<Projectile>(out Projectile proj)) proj.damage = damage;
                if (tear.TryGetComponent<Rigidbody2D>(out Rigidbody2D tearRb)) tearRb.linearVelocity = shotDir * projectileSpeed;
            }
        }

        yield return new WaitForSeconds(0.5f);
    }
}