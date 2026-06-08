using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class ExplodeOnDeath : MonoBehaviour
{
    [Tooltip("Drag your completed Explosion Prefab here!")]
    public GameObject explosionPrefab;

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
        if (explosionPrefab == null) return;

        GameObject boom = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (boom.TryGetComponent(out Explosion explosionScript))
        {
            explosionScript.TriggerExplode();
        }
    }
}