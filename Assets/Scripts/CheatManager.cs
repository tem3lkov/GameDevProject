using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour {
    void Update() {
        if (Keyboard.current.shiftKey.IsPressed() && Keyboard.current.kKey.wasPressedThisFrame) {
            KillAllActiveEnemies();
        }
    }

    private void KillAllActiveEnemies() {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in allEnemies) {
            enemy.Die();
        }

        Debug.Log("Cheat Activated: All enemies killed.");
    }
}