using UnityEngine;
using System.Collections.Generic;

public class BossEncounter : RoomEncounter
{
    [Header("Specific Floor Bosses (Optional)")]
    [Tooltip("If assigned, this boss will always spawn on Floor 1. If empty, a random boss is picked.")]
    public EnemyController floorOneBossPrefab;
    [Tooltip("If assigned, this boss will always spawn on Floor 2. If empty, a random boss is picked.")]
    public EnemyController floorTwoBossPrefab;
    [Tooltip("If assigned, this boss will always spawn on Floor 3. If empty, a random boss is picked.")]
    public EnemyController floorThreeBossPrefab;

    [Header("Random Boss Pool")]
    [Tooltip("Bosses placed here can be randomly picked for any floor that doesn't have a specific boss assigned.")]
    public EnemyController[] randomBossPool;
    public Transform bossSpawnPoint;

    [Header("Boss Specific Spawns")]
    public GameObject trapdoorPrefab;
    public Transform spawnPoint_Trapdoor;
    [Tooltip("The exact center where the boss item should spawn")]
    public Transform spawnCenter;

    private float currentGenerationMaxHealth = 0f;
    private int lastEnemyCount = 0;
    private bool isBossFightActive = false;

    protected override void StartEncounter()
    {
        if (roomLogic.IsInCombat() || hasClearedEncounter) return;

        doorsInRoom = GetComponentsInChildren<Door>();

        roomLogic.EnterCombat();
        SetDoorsLocked(true);
        GameManager.Instance.ChangeState(GameState.engagingBoss);

        if (TryGetComponent<AStarGrid>(out AStarGrid grid))
        {
            grid.InitializeGrid();
        }

        SpawnEnemies();

        EnemyController.TriggerBossUIActive(true);
        isBossFightActive = true;
        lastEnemyCount = activeEnemies.Count;
        CalculateGenerationMaxHealth();
    }

    private void Update()
    {
        if (!isBossFightActive || activeEnemies.Count == 0) return;

        if (activeEnemies.Count > lastEnemyCount)
        {
            CalculateGenerationMaxHealth();
        }

        lastEnemyCount = activeEnemies.Count;

        float currentTotalHealth = 0f;
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null) currentTotalHealth += enemy.GetCurrentHealth();
        }

        if (currentGenerationMaxHealth > 0)
        {
            EnemyController.TriggerBossUIUpdate(currentTotalHealth / currentGenerationMaxHealth);
        }
    }

    private void CalculateGenerationMaxHealth()
    {
        currentGenerationMaxHealth = 0f;
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && enemy.details != null)
            {
                currentGenerationMaxHealth += enemy.details.maxHealth;
            }
        }
    }

    protected override void SpawnEnemies()
    {
        EnemyController bossToSpawn = null;

        if (GameManager.currentLevel == 1 && floorOneBossPrefab != null)
        {
            bossToSpawn = floorOneBossPrefab;
        } else if (GameManager.currentLevel == 2 && floorTwoBossPrefab != null)
        {
            bossToSpawn = floorTwoBossPrefab;
        } else if (GameManager.currentLevel == 3 && floorThreeBossPrefab != null)
        {
            bossToSpawn = floorThreeBossPrefab;
        }

        if (bossToSpawn == null && randomBossPool != null && randomBossPool.Length > 0)
        {
            bossToSpawn = randomBossPool[Random.Range(0, randomBossPool.Length)];
        }

        if (bossToSpawn == null)
        {
            Debug.LogWarning("BossEncounter tried to spawn a boss, but no prefabs were assigned in the Inspector!");
            return;
        }

        Transform spawnPos = bossSpawnPoint != null ? bossSpawnPoint : transform;

        EnemyController spawnedBoss = Instantiate(bossToSpawn, spawnPos.position, Quaternion.identity, transform);

        RegisterNewEnemy(spawnedBoss);
    }

    protected override void EndEncounter()
    {
        base.EndEncounter();

        isBossFightActive = false;
        EnemyController.TriggerBossUIActive(false);

        GameManager.Instance.ChangeState(GameState.levelCompleted);

        if (trapdoorPrefab != null && spawnPoint_Trapdoor != null)
        {
            Instantiate(trapdoorPrefab, spawnPoint_Trapdoor.position, Quaternion.identity, transform);
        }
    }

    protected override void SpawnClearReward()
    {
        Vector2 dropPosition = spawnCenter != null ? spawnCenter.position : transform.position;

        Item bossItem = ItemManager.Instance.SpawnRandomNonResourceItem(dropPosition, false);

        if (bossItem != null)
        {
            bossItem.transform.position = dropPosition;
        }
    }
}