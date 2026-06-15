using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "CardinalShoot", menuName = "Enemy Data/Attacks/Cardinal Shoot (Clotty)")]
public class AttackSO_CardinalShoot : EnemyAttackSO
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 6f;
    public float damage = 1f;

    public float spawnOffset = 0.4f; 

    [Header("Animation Timings")]
    public float prepTime = 0.4f;
    public float recoveryTime = 0.3f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Prep");

        yield return new WaitForSeconds(prepTime);

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Shoot");

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        if (projectilePrefab != null)
        {
            foreach (Vector2 dir in directions)
            {
                Vector2 spawnPos = (Vector2)enemy.transform.position + (dir * spawnOffset);

                GameObject tearObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                if (tearObj.TryGetComponent<Projectile>(out Projectile proj))
                {
                    proj.damage = damage;
                }

                if (tearObj.TryGetComponent<Rigidbody2D>(out Rigidbody2D projRb))
                {
                    projRb.linearVelocity = dir * projectileSpeed;
                }
            }
        }

        yield return new WaitForSeconds(recoveryTime);

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
    }
}