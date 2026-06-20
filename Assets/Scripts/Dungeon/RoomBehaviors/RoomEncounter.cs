using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Room))]
public class RoomEncounter : MonoBehaviour
{
    [Header("Encounter Settings")]
    [SerializeField] private EnemyController[] enemyPrefabsToSpawn;
    [SerializeField] private List<Transform> spawnPoints;

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

        Debug.Log("Locking doors and spawning enemies!");
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
            EnemyController randomEnemy = enemyPrefabsToSpawn[Random.Range(0, enemyPrefabsToSpawn.Length)];

            EnemyController spawnedEnemy = Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity, transform);

            spawnedEnemy.OnDeath += HandleEnemyDeath;
            activeEnemies.Add(spawnedEnemy);
        }
    }
    protected virtual void HandleEnemyDeath(EnemyController deadEnemy)
    {
        deadEnemy.OnDeath -= HandleEnemyDeath;
        activeEnemies.Remove(deadEnemy);

        if (activeEnemies.Count == 0)
        {
            EndEncounter();
        }
    }

    protected virtual void EndEncounter()
    {
        Debug.Log("Open doors");
        hasClearedEncounter = true;
        roomLogic.ExitCombat();
        SetDoorsLocked(false);

        ItemManager.Instance.SpawnCoin(transform.position);
        GameManager.Instance.ChangeState(GameState.playingLevel);
    }

    protected void SetDoorsLocked(bool isLocked)
    {
        foreach (Door door in doorsInRoom)
        {
            if (isLocked) door.EncounterLock();
            else door.EncounterUnlock();
        }
    }
    public void RegisterNewEnemy(EnemyController newEnemy)
    {
        activeEnemies.Add(newEnemy);
        newEnemy.OnDeath += HandleEnemyDeath;
    }
}