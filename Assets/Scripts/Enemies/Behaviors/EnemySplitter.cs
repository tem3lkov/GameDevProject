using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemySplitter : MonoBehaviour
{
    [Header("Split Settings")]
    [Tooltip("The smaller enemy prefab to spawn.")]
    public EnemyController smallerEnemyPrefab;

    public int spawnCount = 2;

    [Tooltip("With what force should they disperse at spawn")]
    public float splitForce = 4f;

    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void OnEnable()
    {
        if (enemyController != null)
            enemyController.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (enemyController != null)
            enemyController.OnDeath -= HandleDeath;
    }

    private void HandleDeath(EnemyController deadEnemy)
    {
        if (smallerEnemyPrefab == null) return;

        RoomEncounter encounter = GetComponentInParent<RoomEncounter>();

        float angleStep = 360f / spawnCount;
        float randomOffset = Random.Range(0f, 360f);

        for (int i = 0; i < spawnCount; i++)
        {
            float angle = (i * angleStep + randomOffset) * Mathf.Deg2Rad;
            Vector2 pushDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector3 spawnPosition = transform.position + (Vector3)pushDir * 0.3f;

            EnemyController cloneController = Instantiate(smallerEnemyPrefab, spawnPosition, Quaternion.identity, transform.parent);

            if (cloneController.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.AddForce(pushDir * splitForce, ForceMode2D.Impulse);
            }

            if (encounter != null && cloneController != null)
            {
                encounter.RegisterNewEnemy(cloneController);
            }
        }
    }
}