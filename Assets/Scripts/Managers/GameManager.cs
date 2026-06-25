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
    levelAdvance,
    gameWon,
    gameLost,
    gamePaused,
    dungeonOverviewMap,
    restartGame
}

public enum StartMode
{
    NewGame,
    Continue,
    AdvanceFloor
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Header("Game State")]
    public GameState currentState;
    public StartMode PendingStartMode;
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
        
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        
        ResetSeed(customPlayerSeed);
    }

    private void Start()
    {
        ChangeState(GameState.gameStarted);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;

        switch (PendingStartMode)
        {
            case StartMode.NewGame:
                ResetGameVariables(customPlayerSeed);
                SaveRunData();
                break;

            case StartMode.Continue:
                if (SaveManager.Instance.SaveExists()) {
                    LoadRunData();
                }
                else {
                    ResetGameVariables(customPlayerSeed);
                    SaveRunData();
                }
                break;

            case StartMode.AdvanceFloor:
                LoadRunData();
                break;
        }

        GameManager.Instance.currentState = GameState.playingLevel;
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
                Time.timeScale = 1f;
                break;

            case GameState.playingLevel:
                Time.timeScale = 1f;
                break;

            case GameState.engagingEnemies:
            case GameState.engagingBoss:
                Time.timeScale = 1f;
                break;

            case GameState.gamePaused:
                Time.timeScale = 0f;
                break;

            case GameState.gameLost:
                SaveManager.Instance.DeleteSave();
                ReturnToMainMenu();
                break;

            case GameState.levelCompleted:
                Debug.Log("Level complete! Waiting for player to use trapdoor...");
                break;

            case GameState.levelAdvance:
                AdvanceLevel();
                break;

            case GameState.gameWon:
                SaveManager.Instance.DeleteSave();
                Debug.Log("🏆 CONGRATULATIONS! YOU ESCAPED THE DUNGEON! 🏆");
                ReturnToMainMenu();
                break;

            case GameState.restartGame:
                NewGame();
                break;
        }
    }

    private void NewGame(int specificSeed = 0)
    {
        PendingStartMode = StartMode.NewGame;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Continue()
    {
        if (!SaveManager.Instance.SaveExists())
        {
            Debug.Log("No save file found. Starting a new game instead.");
            NewGame();
            return;
        }
        PendingStartMode = StartMode.Continue;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void AdvanceLevel()
    {
        if (currentLevel < maxLevels)
        {
            currentLevel++;
            OnLevelChanged?.Invoke(currentLevel);

            SaveRunData();
            PendingStartMode = StartMode.AdvanceFloor;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else
        {
            ChangeState(GameState.gameWon);
        }
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetGameVariables(int specificSeed = 0)
    {
        GameManager.Instance.currentLevel = 1;
        ResetSeed(specificSeed);

        PlayerHealth.Instance.ResetHealth();
        PlayerInventory.Instance.ResetInventory();
    }

    private int ResetSeed(int specificSeed = 0)
    {
        if (specificSeed != 0)
        {
            GameManager.Instance.customPlayerSeed = specificSeed;
        } else
        {
            GameManager.Instance.customPlayerSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        int floorSeed = GameManager.Instance.customPlayerSeed + GameManager.Instance.currentLevel;

        UnityEngine.Random.InitState(floorSeed);
        RunRNG.InitializeSeed((uint)Mathf.Abs(floorSeed));

        return floorSeed;
    }

    private void SaveRunData() // Object parameters -> CurrentRun data -> File serialization
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

        SaveManager.Instance.Save(CurrentRun); // Serialize
    }

    private void LoadRunData() // File deserialization -> CurrentRun data -> Object parameters
    {
        if (!SaveManager.Instance.SaveExists()) return;
        CurrentRun = SaveManager.Instance.Load(); // Deserialize

        GameManager.Instance.currentLevel = CurrentRun.currentLevel;
        ResetSeed(CurrentRun.runSeed);

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