using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Room))]
public class RoomEncounter : MonoBehaviour {
    [Header("Encounter Settings")]
    [SerializeField] private Enemy[] enemyPrefabsToSpawn;
    [SerializeField] private Transform[] spawnPoints; 

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
        if (roomLogic.GetRoomType() == RoomType.Item || roomLogic.GetRoomType() == RoomType.Shop ||
            roomLogic.GetRoomType() == RoomType.Secret || roomLogic.GetRoomType() == RoomType.Boss) {
            return;
        }

        if (enemyPrefabsToSpawn.Length == 0) return;

        doorsInRoom = GetComponentsInChildren<Door>();

        roomLogic.EnterCombat();
        SetDoorsLocked(true);
        SpawnEnemies();
    }

    protected virtual void SpawnEnemies() {
        int maxEnemies = Mathf.Min(4, spawnPoints.Length + 1);
        int enemiesToSpawn = Random.Range(1, maxEnemies);

        List<Transform> availableSpawns = new List<Transform>(spawnPoints);

        for (int i = 0; i < enemiesToSpawn; i++) {
            int randomIndex = Random.Range(0, availableSpawns.Count);
            Transform spawnPoint = availableSpawns[randomIndex];

            availableSpawns.RemoveAt(randomIndex);

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
        ItemManager.instance.SpawnCoin(transform.position);
    }

    protected void SetDoorsLocked(bool isLocked) {
        foreach (Door door in doorsInRoom) {
            if (isLocked) door.EncounterLock();
            else door.EncounterUnlock();
        }
    }
}