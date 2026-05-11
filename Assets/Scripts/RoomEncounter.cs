using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Room))]
public class RoomEncounter : MonoBehaviour {
    [Header("Encounter Settings")]
    public Enemy[] enemyPrefabsToSpawn;
    public Transform[] spawnPoints; 

    protected Room roomLogic;
    protected List<Enemy> activeEnemies = new List<Enemy>();
    protected Door[] doorsInRoom;

    protected virtual void Awake() {
        roomLogic = GetComponent<Room>();
    }

    protected virtual void OnEnable() {
        roomLogic.OnPlayerEnteredRoom += StartEncounter;
    }

    protected virtual void OnDisable() {
        roomLogic.OnPlayerEnteredRoom -= StartEncounter;
    }

    protected virtual void StartEncounter() {
        if (roomLogic.roomType == RoomType.Item || roomLogic.roomType == RoomType.Shop ||
            roomLogic.roomType == RoomType.Secret || roomLogic.roomType == RoomType.Boss) {
            return;
        }

        if (enemyPrefabsToSpawn.Length == 0) return;

        doorsInRoom = GetComponentsInChildren<Door>();

        roomLogic.EnterCombat();
        SetDoorsLocked(true);
        SpawnEnemies();
    }

    protected virtual void SpawnEnemies() {
        int enemiesToSpawn = Random.Range(1, 4);

        for (int i = 0; i < enemiesToSpawn; i++) {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Enemy randomEnemy = enemyPrefabsToSpawn[Random.Range(0, enemyPrefabsToSpawn.Length)];

            Enemy spawnedEnemy = Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity, transform);

            spawnedEnemy.OnDeath += HandleEnemyDeath;
            activeEnemies.Add(spawnedEnemy);
        }
    }

    protected virtual void HandleEnemyDeath(Enemy deadEnemy) {
        deadEnemy.OnDeath -= HandleEnemyDeath;
        activeEnemies.Remove(deadEnemy);

        if (activeEnemies.Count == 0) {
            EndEncounter();
        }
    }

    protected virtual void EndEncounter() {
        Debug.Log("Open doors");
        roomLogic.ExitCombat();
        SetDoorsLocked(false);
    }

    protected void SetDoorsLocked(bool isLocked) {
        foreach (Door door in doorsInRoom) {
            if (isLocked) door.EncounterLock();
            else door.EncounterUnlock();
        }
    }
}