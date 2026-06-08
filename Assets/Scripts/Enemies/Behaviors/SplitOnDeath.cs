using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class SplitOnDeath : MonoBehaviour
{
    [Header("Split Settings")]
    [Tooltip("The smaller enemy prefab to spawn.")]
    public GameObject smallerEnemyPrefab;
    public int spawnCount = 2;
    public float spawnRadius = 0.5f;

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

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;

            GameObject clone = Instantiate(smallerEnemyPrefab, spawnPosition, Quaternion.identity, transform.parent);

            EnemyController cloneController = clone.GetComponent<EnemyController>();

            if (encounter != null && cloneController != null)
            {
                encounter.RegisterNewEnemy(cloneController);
            }
        }
    }
}