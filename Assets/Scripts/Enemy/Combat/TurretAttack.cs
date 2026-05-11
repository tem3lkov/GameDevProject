using System.Collections;
using UnityEngine;

public class TurretAttack : EnemyAttack {
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float damage = 1f;

    public override IEnumerator Execute(Enemy user) {
        user.CanMove = false;
        user.Rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);

        if (projectilePrefab != null && user.Target != null) {
            Vector2 direction = (user.Target.position - transform.position).normalized;
            GameObject tear = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            if (tear.TryGetComponent<Projectile>(out Projectile proj)) proj.damage = damage;
            if (tear.TryGetComponent<Rigidbody2D>(out Rigidbody2D projRb)) projRb.linearVelocity = direction * projectileSpeed;
        }

        yield return new WaitForSeconds(0.5f);

        user.CanMove = true;
    }
}