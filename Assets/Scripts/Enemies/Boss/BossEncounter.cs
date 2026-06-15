using UnityEngine;
using System.Collections.Generic;

public class BossEncounter : RoomEncounter
{
    [Header("Boss Settings")]
    public EnemyController floorOneBossPrefab;
    public EnemyController[] randomBossPool;
    public Transform bossSpawnPoint;

    [Header("Room Rewards")]
    public GameObject trapdoorPrefab;
    public Transform trapdoorSpawnPoint;

    public static int currentFloor = 1;

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
        EnemyController bossToSpawn = (currentFloor == 1 && floorOneBossPrefab != null)
            ? floorOneBossPrefab
            : randomBossPool[Random.Range(0, randomBossPool.Length)];

        if (bossToSpawn == null) return;

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

        if (trapdoorPrefab != null && trapdoorSpawnPoint != null)
        {
            Instantiate(trapdoorPrefab, trapdoorSpawnPoint.position, Quaternion.identity, transform);
        }
    }
}