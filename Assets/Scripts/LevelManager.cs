using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int maxLevels = 3;

    public static event Action<int> OnLevelChanged;

    private void Awake() {
        if (Instance == null) {
            Instance = this;

            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void AdvanceLevel() {
        if (currentLevel < maxLevels) {
            currentLevel++;

            OnLevelChanged?.Invoke(currentLevel);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else {
            Debug.Log("Game Won! Max levels reached.");
        }
    }
}