using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement; // Added for Scene Loading

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
    public static int currentLevel = 1;
    public int maxLevels = 3;
    public static event Action<int> OnLevelChanged;

    private RunData CurrentRun = new();

    protected override void Awake()
    {
        base.Awake();

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.gameStarted);
    }

    
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Room.OnRoomEnteredGlobal += OnRoomEntered;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Room.OnRoomEnteredGlobal -= OnRoomEntered;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentState != GameState.gameStarted)
        {
            LoadRunData();
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
                // FIX: Big console log when you beat the last level
                Debug.Log("🏆 CONGRATULATIONS! YOU ESCAPED THE DUNGEON! 🏆");
                Time.timeScale = 0f; // Stops the game
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

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else
        {
            ChangeState(GameState.gameWon);
        }
    }
    public void ResetGame()
    {
        currentLevel = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SaveRunData()
    {
        CurrentRun.maxHealth = PlayerHealth.Instance.globalMaxRedHalves;
        CurrentRun.redHealth = PlayerHealth.Instance.globalCurrentRedHalves;
        CurrentRun.blueHealth = PlayerHealth.Instance.globalCurrentBlueHalves;

        CurrentRun.bombs = PlayerInventory.Instance.bombs;
        CurrentRun.keys = PlayerInventory.Instance.keys;
        CurrentRun.coins = PlayerInventory.Instance.coins;

        if (PlayerInventory.Instance.GetActiveItem() != null)
            CurrentRun.activeItemID = PlayerInventory.Instance.GetActiveItem().itemName;
        CurrentRun.passiveItemIDs = PlayerInventory.Instance.GetPassiveItemNames();

        Debug.Log("Save successful");
    }

    public void LoadRunData()
    {
        PlayerHealth.Instance.SetMaxHP(CurrentRun.maxHealth);
        PlayerHealth.Instance.SetRedHP(CurrentRun.redHealth);
        PlayerHealth.Instance.SetBlueHP(CurrentRun.blueHealth);

        PlayerInventory.Instance.SetBombs(CurrentRun.bombs);
        PlayerInventory.Instance.SetKeys(CurrentRun.keys);
        PlayerInventory.Instance.SetCoins(CurrentRun.coins);

        if (CurrentRun.activeItemID != "")
            PlayerInventory.Instance.SetActiveItem(CurrentRun.activeItemID);
        PlayerInventory.Instance.SetPassiveItems(CurrentRun.passiveItemIDs);

        Debug.Log("Load successful");
    }
}