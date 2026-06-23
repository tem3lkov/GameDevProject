using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;// TODO remove (just for testing)

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
    public bool continueRun = false;

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
        LoadRunData();
        if (!continueRun && currentState == GameState.gameStarted)
        {
            continueRun = true;
            NewGame(customPlayerSeed);
        }
    }
    
    private void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            Debug.Log("New Game (Reset params) (Save params and serialize)");
            NewGame();// for retrying SAME seed do (customPlayerSeed)
        }
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            Debug.Log("Continue Game (Load - deserialize)");
            Continue();
        }
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            Debug.Log("Advance Level (Save params and serialize) (Load - deserialize)");
            AdvanceLevel();
        }
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
                break;

            case GameState.gamePaused:
                Time.timeScale = 0f;
                break;

            case GameState.gameLost:
                SaveManager.Instance.DeleteSave();
                Time.timeScale = 0f;
                break;

            case GameState.levelCompleted:
                Debug.Log("Level complete! Waiting for player to use trapdoor...");
                break;

            case GameState.gameWon:
                Debug.Log("🏆 CONGRATULATIONS! YOU ESCAPED THE DUNGEON! 🏆");
                SaveManager.Instance.DeleteSave();
                Time.timeScale = 0f;
                break;

            case GameState.restartGame:
                NewGame();
                break;
        }
    }

    public void NewGame(int specificSeed = 0)
    {
        ResetGameVariables(specificSeed);
        SaveRunData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Continue()
    {
        if (!SaveManager.Instance.SaveExists())
        {
            Debug.Log("No save file found.");
            return;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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