using System.Collections;
using UnityEngine;

public class Attack_MonstroSpit : EnemyAttack {
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public int projectileCount = 8;

    [Tooltip("Offset to make tears come out of the mouth area")]
    public Vector3 spawnOffset = new Vector3(0, 0.8f, 0);

    [Tooltip("How far forward to push the tear so it clears the boss's collider")]
    public float spawnDistance = 1.0f; // Added this!

    [Header("Speed Settings")]
    public float minSpeed = 3f;
    public float maxSpeed = 5f;
    public float spreadAngle = 30f;

    public override IEnumerator Execute(Enemy enemy) {
        if (enemy == null) yield break;

        enemy.Rb.linearVelocity = Vector2.zero;

        if (enemy.Anim != null) enemy.Anim.SetTrigger("Spit");
        yield return new WaitForSeconds(0.5f);

        if (enemy.Target != null && projectilePrefab != null) {
            Vector3 mouthPos = transform.position + spawnOffset;
            Vector2 dirToPlayer = (enemy.Target.position - mouthPos).normalized;

            for (int i = 0; i < projectileCount; i++) {
                if (enemy.Target == null) break;

                float randomAngle = Random.Range(-spreadAngle, spreadAngle);
                Vector2 finalDir = Quaternion.Euler(0, 0, randomAngle) * dirToPlayer;

                Vector3 finalSpawnPos = mouthPos + (Vector3)(finalDir * spawnDistance);

                GameObject tear = Instantiate(projectilePrefab, finalSpawnPos, Quaternion.identity);

                if (tear.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) {
                    rb.linearVelocity = finalDir * Random.Range(minSpeed, maxSpeed);
                }

                yield return new WaitForSeconds(0.02f);
            }
        }

        yield return new WaitForSeconds(0.5f);
        if (enemy.Anim != null) enemy.Anim.SetTrigger("Idle");
    }
}