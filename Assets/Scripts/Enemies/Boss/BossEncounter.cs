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
    }

    protected override void SpawnEnemies()
    {
        EnemyController bossToSpawn = (currentFloor == 1 && floorOneBossPrefab != null)
            ? floorOneBossPrefab
            : randomBossPool[Random.Range(0, randomBossPool.Length)];

        if (bossToSpawn == null) return;

        Transform spawnPos = bossSpawnPoint != null ? bossSpawnPoint : transform;

        EnemyController spawnedBoss = Instantiate(bossToSpawn, spawnPos.position, Quaternion.identity, transform);
        spawnedBoss.OnDeath += HandleEnemyDeath;
        activeEnemies.Add(spawnedBoss);
    }

    protected override void EndEncounter()
    {
        base.EndEncounter();

        GameManager.Instance.ChangeState(GameState.levelCompleted);

        if (trapdoorPrefab != null && trapdoorSpawnPoint != null)
        {
            Instantiate(trapdoorPrefab, trapdoorSpawnPoint.position, Quaternion.identity, transform);
        }
    }
}