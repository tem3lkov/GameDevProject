using UnityEngine;

// Inherits from RoomEncounter to steal all its good logic!
public class BossEncounter : RoomEncounter {

    [Header("Boss Settings")]
    public Enemy floorOneBossPrefab;

    public GameObject trapdoorPrefab;
    public Transform trapdoorSpawnPoint;

    public Enemy[] randomBossPool;

    public Transform bossSpawnPoint;


    public static int currentFloor = 1;

    protected override void StartEncounter() {
        if (roomLogic.GetRoomType() != RoomType.Boss) return;
        doorsInRoom = GetComponentsInChildren<Door>();

        roomLogic.EnterCombat();
        SetDoorsLocked(true);
        SpawnEnemies();
    }

    protected override void SpawnEnemies() {
        Enemy bossToSpawn = null;

        if (currentFloor == 1 && floorOneBossPrefab != null) {
            bossToSpawn = floorOneBossPrefab;
        } else if (randomBossPool.Length > 0) {
            int randomIndex = Random.Range(0, randomBossPool.Length);
            bossToSpawn = randomBossPool[randomIndex];
        }

        if (bossToSpawn == null) {
            Debug.LogWarning("No boss prefabs assigned in BossEncounter!");
            return;
        }
        Debug.Log("Boss spawned");

        Transform spawnPos = bossSpawnPoint != null ? bossSpawnPoint : transform;
        Enemy spawnedBoss = Instantiate(bossToSpawn, spawnPos.position, Quaternion.identity, transform);

        spawnedBoss.OnDeath += HandleEnemyDeath;
        activeEnemies.Add(spawnedBoss);
    }

    protected override void EndEncounter() {
        base.EndEncounter();

        if (trapdoorPrefab != null) {
            Instantiate(trapdoorPrefab, trapdoorSpawnPoint.position, Quaternion.identity);
        }
    }
}