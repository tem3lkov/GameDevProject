using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Room))]
public class RoomEncounter : MonoBehaviour
{
    [Header("Encounter Settings")]
    [SerializeField] private EnemyController[] enemyPrefabsToSpawn;
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Reward Probabilities (Should sum to 100)")]
    [Tooltip("Percentage chance to drop a resource (Coin, Bomb, Key)")]
    [SerializeField][Range(0, 100)] private int resourceDropChance = 50;
    [Tooltip("Percentage chance to drop a Normal Chest")]
    [SerializeField][Range(0, 100)] private int normalChestChance = 40;
    [Tooltip("Percentage chance to drop a Locked Chest")]
    [SerializeField][Range(0, 100)] private int lockedChestChance = 10;

    protected Room roomLogic;
    protected List<EnemyController> activeEnemies = new List<EnemyController>();
    protected Door[] doorsInRoom;

    protected bool hasClearedEncounter = false;

    protected virtual void Awake()
    {
        roomLogic = GetComponent<Room>();
    }

    protected virtual void OnEnable()
    {
        roomLogic.OnPlayerFirstEnteredRoom += StartEncounter;
    }

    protected virtual void OnDisable()
    {
        roomLogic.OnPlayerFirstEnteredRoom -= StartEncounter;
    }

    protected virtual void StartEncounter()
    {
        if (roomLogic.GetRoomIndex() == 45) return;

        if (hasClearedEncounter || roomLogic.IsInCombat()) return;
        if (enemyPrefabsToSpawn.Length == 0) return;

        doorsInRoom = GetComponentsInChildren<Door>();

        roomLogic.EnterCombat();
        SetDoorsLocked(true);

        GameManager.Instance.ChangeState(GameState.engagingEnemies);

        if (TryGetComponent<AStarGrid>(out AStarGrid grid))
        {
            grid.InitializeGrid();
        }

        SpawnEnemies();
    }

    protected virtual void SpawnEnemies()
    {
        if (spawnPoints == null || spawnPoints.Count == 0 || enemyPrefabsToSpawn.Length == 0) return;

        foreach (Transform spawnPoint in spawnPoints)
        {
            EnemyController randomEnemy = enemyPrefabsToSpawn[RunRNG.Range(0, enemyPrefabsToSpawn.Length)];
            EnemyController spawnedEnemy = Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity, transform);

            RegisterNewEnemy(spawnedEnemy);
        }
    }

    public void RegisterNewEnemy(EnemyController enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            enemy.OnDeath += HandleEnemyDeath;
            activeEnemies.Add(enemy);
        }
    }

    protected virtual void HandleEnemyDeath(EnemyController deadEnemy)
    {
        deadEnemy.OnDeath -= HandleEnemyDeath;
        activeEnemies.Remove(deadEnemy);

        StartCoroutine(CheckEncounterEndRoutine());
    }

    private System.Collections.IEnumerator CheckEncounterEndRoutine()
    {
        yield return new WaitForEndOfFrame();

        if (activeEnemies.Count == 0 && !hasClearedEncounter)
        {
            EndEncounter();
        }
    }

    protected virtual void EndEncounter()
    {
        hasClearedEncounter = true;
        roomLogic.ExitCombat();
        SetDoorsLocked(false);

        SpawnClearReward();

        GameManager.Instance.ChangeState(GameState.playingLevel);
    }

    protected virtual void SpawnClearReward()
    {
        ItemManager.Instance.SpawnRoomClearReward(
            transform.position,
            resourceDropChance,
            normalChestChance,
            lockedChestChance
        );
    }

    protected void SetDoorsLocked(bool isLocked)
    {
        if (doorsInRoom == null) return;
        foreach (Door door in doorsInRoom)
        {
            if (door != null)
            {
                if (isLocked) door.EncounterLock();
                else door.EncounterUnlock();
            }
        }
    }
}