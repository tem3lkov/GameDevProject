using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum GameState
{
    gameStarted,
    playingLevel,
    engagingEnemies,
    bossStage,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    gamePaused,
    dungeonOverviewMap,
    restartGame
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Header("Game State")]
    public GameState currentState;
    public static event Action<GameState> OnGameStateChanged;

    [Header("Level Progression")]
    public int currentLevel = 1;
    public int maxLevels = 3;
    public static event Action<int> OnLevelChanged;

    [Header("Run Information")]
    [Tooltip("Leave at 0 to generate a random seed. Enter a number to play a specific seed!")]
    public int customPlayerSeed = 0;

    private RunData CurrentRun = new();

    public int GetCurrentSeed()
    {
        return CurrentRun.runSeed;
    }

    protected override void Awake()
    {
        base.Awake();
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        if (customPlayerSeed != 0)
        {
            CurrentRun.runSeed = customPlayerSeed;
        } else
        {
            CurrentRun.runSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        int floorSeed = CurrentRun.runSeed + currentLevel;

        UnityEngine.Random.InitState(floorSeed);

        RunRNG.InitializeSeed((uint)Mathf.Abs(floorSeed));
    }

    private void Start()
    {
        ChangeState(GameState.gameStarted);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Room.OnRoomEnteredGlobal += OnRoomEntered;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Room.OnRoomEnteredGlobal -= OnRoomEntered;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentState != GameState.gameStarted)
        {
            LoadRunData();

            int floorSeed = CurrentRun.runSeed + currentLevel;
            RunRNG.InitializeSeed((uint)floorSeed);

            Debug.Log($"[Seeded Run] Initialized Level {currentLevel} with Floor Seed: {floorSeed}");
        }
    }

    private void OnRoomEntered(Room room)
    {
        SaveRunData();
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log($"Game State Changed: {newState}");

        HandleStateChange(newState);
        OnGameStateChanged?.Invoke(currentState);
    }

    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.gameStarted:
                Debug.Log($"[Seeded Run] Started new run with Master Seed: {CurrentRun.runSeed}");
                SaveRunData();
                Time.timeScale = 1f;
                break;

            case GameState.playingLevel:
                SaveRunData();
                Time.timeScale = 1f;
                break;

            case GameState.engagingEnemies:
            case GameState.engagingBoss:
                SaveRunData();
                break;

            case GameState.gamePaused:
            case GameState.gameLost:
                Time.timeScale = 0f;
                break;

            case GameState.levelCompleted:
                Debug.Log("Level complete! Waiting for player to use trapdoor...");
                break;

            case GameState.gameWon:
                Debug.Log("🏆 CONGRATULATIONS! YOU ESCAPED THE DUNGEON! 🏆");
                Time.timeScale = 0f;
                break;

            case GameState.restartGame:
                currentLevel = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }

    public void AdvanceLevel()
    {
        if (currentLevel < maxLevels)
        {
            currentLevel++;
            OnLevelChanged?.Invoke(currentLevel);
            SaveRunData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else
        {
            ChangeState(GameState.gameWon);
        }
    }

    public void ResetGame()
    {
        currentLevel = 1;
        customPlayerSeed = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveRunData()
    {
        CurrentRun.currentLevel = GameManager.Instance.currentLevel;
        CurrentRun.runSeed = GameManager.Instance.customPlayerSeed;

        CurrentRun.maxHealth = PlayerHealth.Instance.globalMaxRedHalves;
        CurrentRun.redHealth = PlayerHealth.Instance.globalCurrentRedHalves;
        CurrentRun.blueHealth = PlayerHealth.Instance.globalCurrentBlueHalves;

        CurrentRun.bombs = PlayerInventory.Instance.bombs;
        CurrentRun.keys = PlayerInventory.Instance.keys;
        CurrentRun.coins = PlayerInventory.Instance.coins;

        if (PlayerInventory.Instance.GetActiveItem() != null)
            CurrentRun.activeItemID = PlayerInventory.Instance.GetActiveItem().itemName;

        CurrentRun.passiveItemIDs = PlayerInventory.Instance.GetPassiveItemNames();
    }

    public void LoadRunData()
    {
        GameManager.Instance.currentLevel = CurrentRun.currentLevel;
        GameManager.Instance.customPlayerSeed = CurrentRun.runSeed;

        PlayerHealth.Instance.SetMaxHP(CurrentRun.maxHealth);
        PlayerHealth.Instance.SetRedHP(CurrentRun.redHealth);
        PlayerHealth.Instance.SetBlueHP(CurrentRun.blueHealth);

        PlayerInventory.Instance.SetBombs(CurrentRun.bombs);
        PlayerInventory.Instance.SetKeys(CurrentRun.keys);
        PlayerInventory.Instance.SetCoins(CurrentRun.coins);

        if (!string.IsNullOrEmpty(CurrentRun.activeItemID))
            PlayerInventory.Instance.SetActiveItem(CurrentRun.activeItemID);

        PlayerInventory.Instance.SetPassiveItems(CurrentRun.passiveItemIDs);
    }
}