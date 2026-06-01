using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretShoot", menuName = "Enemy Data/Attacks/Turret Shoot")]
public class AttackSO_Turret : EnemyAttackSO
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
    public float damage = 1f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Shoot");

        yield return new WaitForSeconds(0.2f);

        if (projectilePrefab != null && enemy.Target != null)
        {
            Vector2 direction = (enemy.Target.position - enemy.transform.position).normalized;
            GameObject tear = Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);

            if (tear.TryGetComponent(out Projectile proj)) proj.damage = damage;
            if (tear.TryGetComponent(out Rigidbody2D projRb)) projRb.linearVelocity = direction * projectileSpeed;
        } else if (projectilePrefab == null)
        {
            Debug.LogWarning($"{enemy.gameObject.name} tried to shoot, but Projectile Prefab is missing in the SO!");
        }

        yield return new WaitForSeconds(0.5f);
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
    }
}