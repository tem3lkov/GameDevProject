using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Room))]
public class RoomEncounter : MonoBehaviour {
    [Header("Encounter Settings")]
    [SerializeField] private Enemy[] enemyPrefabsToSpawn;
    [SerializeField] private Transform[] spawnPoints; 

    [SerializeField] private CoinScriptable[] coinData = new CoinScriptable[3];
    private Room roomLogic;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private Door[] doorsInRoom;

    private void Awake() {
        roomLogic = GetComponent<Room>();
    }

    private void OnEnable() {
        roomLogic.OnPlayerEnteredRoom += StartEncounter;
    }

    private void OnDisable() {
        roomLogic.OnPlayerEnteredRoom -= StartEncounter;
    }

    private void StartEncounter() {
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

    private void SpawnEnemies() {
        int enemiesToSpawn = Random.Range(1, 4);

        for (int i = 0; i < enemiesToSpawn; i++) {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Enemy randomEnemy = enemyPrefabsToSpawn[Random.Range(0, enemyPrefabsToSpawn.Length)];

            Enemy spawnedEnemy = Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity, transform);

            spawnedEnemy.OnDeath += HandleEnemyDeath;
            activeEnemies.Add(spawnedEnemy);
        }
    }

    private void HandleEnemyDeath(Enemy deadEnemy) {
        deadEnemy.OnDeath -= HandleEnemyDeath;
        activeEnemies.Remove(deadEnemy);

        if (activeEnemies.Count == 0)
        {
            EndEncounter();
        }
    }

    private void EndEncounter() {
        Debug.Log("Open doors");
        roomLogic.ExitCombat();
        SetDoorsLocked(false);
        SpawnCoin();
    }

    private void SetDoorsLocked(bool isLocked) {
        foreach (Door door in doorsInRoom) {
            if (isLocked) door.EncounterLock();
            else door.EncounterUnlock();
        }
    }

    private void SpawnCoin()
    {
        int chosenCoin = Random.Range(1, 10);
        int coinIndex;
        switch (chosenCoin)
        {
            case 8:
            case 9:
                coinIndex = 1; // Uncommon coin
                break;
            case 10:
                coinIndex = 2; // Rare coin
                break;
            default:
                coinIndex = 0; // Default to common coin
                break;
        }
        CoinScriptable coinToSpawn = coinData[coinIndex];        
        
        var newCoin = Instantiate(RoomManager.instance.coinPrefab, transform.position, Quaternion.identity);
        newCoin.Initialize(coinToSpawn);
    }
}