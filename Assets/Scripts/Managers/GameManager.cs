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

    private void Start()
    {
        ChangeState(GameState.gameStarted);
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
                Time.timeScale = 1f;
                break;

            case GameState.engagingEnemies:
            case GameState.engagingBoss:
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
}