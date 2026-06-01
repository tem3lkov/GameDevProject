using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour {
    void Update() {
        if (Keyboard.current.shiftKey.IsPressed() && Keyboard.current.kKey.wasPressedThisFrame) {
            KillAllActiveEnemies();
        }
    }

    private void KillAllActiveEnemies() {
        EnemyController[] allEnemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (EnemyController enemy in allEnemies) {
            enemy.Die();
        }

        Debug.Log("Cheat Activated: All enemies killed.");
    }
}