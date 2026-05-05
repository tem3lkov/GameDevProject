using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class TurretAttack : MonoBehaviour {
    [Header("Attack Settings")]
    public GameObject projectilePrefab;

    [Header("Randomized Timing")]
    [Tooltip("The shortest possible time between shots")]
    public float minCooldown = 0.8f;
    [Tooltip("The longest possible time between shots")]
    public float maxCooldown = 2.5f;

    private Enemy enemy;
    private float nextAttackTime;

    private void Awake() {
        enemy = GetComponent<Enemy>();
    }

    private void Start() {
        SetNextAttackTime();
    }

    private void Update() {
        if (enemy.CurrentState == EnemyState.Aggro && enemy.Target != null) {

            if (Time.time >= nextAttackTime) {
                Shoot();
                SetNextAttackTime();
            }
        }
    }

    private void Shoot() {
        if (projectilePrefab == null) return;

        Vector2 direction = (enemy.Target.position - transform.position).normalized;
        GameObject tear = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        if (tear.TryGetComponent<Rigidbody2D>(out Rigidbody2D projRb)) {
            projRb.linearVelocity = direction * 5f;
        }
    }

    private void SetNextAttackTime() {
        float randomDelay = Random.Range(minCooldown, maxCooldown);
        nextAttackTime = Time.time + randomDelay;
    }
}