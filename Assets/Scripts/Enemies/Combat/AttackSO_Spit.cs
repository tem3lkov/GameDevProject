using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SpitAttack", menuName = "Enemy Data/Attacks/Monstro Spit")]
public class AttackSO_Spit : EnemyAttackSO
{
    public GameObject projectilePrefab;
    public int projectileCount = 8;
    public Vector3 spawnOffset = new Vector3(0, 0.8f, 0);
    public float minSpeed = 3f, maxSpeed = 5f, spreadAngle = 30f;

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Spit");
        yield return new WaitForSeconds(0.2f);

        if (enemy.Target == null) yield break;

        if (projectilePrefab != null)
        {
            Vector3 mouthPos = enemy.transform.position + spawnOffset;
            Vector2 dirToPlayer = (enemy.Target.position - mouthPos).normalized;

            for (int i = 0; i < projectileCount; i++)
            {
                float randomAngle = Random.Range(-spreadAngle, spreadAngle);
                Vector2 finalDir = Quaternion.Euler(0, 0, randomAngle) * dirToPlayer;
                Vector3 finalSpawnPos = mouthPos + (Vector3)(finalDir * 1.0f);

                GameObject tear = Instantiate(projectilePrefab, finalSpawnPos, Quaternion.identity);
                if (tear.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.linearVelocity = finalDir * Random.Range(minSpeed, maxSpeed);
                }
            }
        } else
        {
            Debug.LogWarning($"{enemy.name} tried to spit, but projectilePrefab is missing in the Inspector!");
        }

        yield return new WaitForSeconds(0.5f);
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
    }
}