using System.Collections;
using UnityEngine;

public class Attack_RadialShoot : EnemyAttack {
    public GameObject projectilePrefab;
    public int projectileCount = 8;
    public float projectileSpeed = 6f;
    public float damage = 1f;

    public override IEnumerator Execute(Enemy user) {
        user.CanMove = false;
        user.Rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        if (projectilePrefab != null) {
            float angleStep = 360f / projectileCount;
            for (int i = 0; i < projectileCount; i++) {
                float angle = i * angleStep;
                Vector2 shotDir = Quaternion.Euler(0, 0, angle) * Vector2.up;

                GameObject tear = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                if (tear.TryGetComponent<Projectile>(out Projectile proj)) proj.damage = damage;
                if (tear.TryGetComponent<Rigidbody2D>(out Rigidbody2D tearRb)) tearRb.linearVelocity = shotDir * projectileSpeed;
            }
        }

        yield return new WaitForSeconds(0.5f);

        user.CanMove = true;
    }
}