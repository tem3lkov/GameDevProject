using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemySplitter : MonoBehaviour
{
    [Header("Split Settings")]
    [Tooltip("The smaller enemy prefab to spawn.")]
    public EnemyController smallerEnemyPrefab;

    public int spawnCount = 2;

    [Tooltip("With what speed should they burst out")]
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

        Vector2[] diagonalDirs = new Vector2[] {
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized, 
            new Vector2(-1, -1).normalized,
            new Vector2(1, -1).normalized  
        };

        int startIndex = Random.Range(0, 4);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 pushDir = diagonalDirs[(startIndex + i) % 4];
            Vector3 spawnPosition = transform.position + (Vector3)pushDir * 0.3f;

            EnemyController cloneController = Instantiate(smallerEnemyPrefab, spawnPosition, Quaternion.identity, transform.parent);

            if (encounter != null)
            {
                encounter.RegisterNewEnemy(cloneController);
            }

            if (cloneController.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearVelocity = pushDir * splitForce;
            }
        }
    }
}