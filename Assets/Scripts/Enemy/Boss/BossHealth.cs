using UnityEngine;
using System;

public class BossHealth : MonoBehaviour, IDamageable {
    public float maxHealth = 100f;
    private float currentHealth;

    public static event Action<float, float> OnBossHealthChanged;
    public static event Action<bool> OnBossFightToggled;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void StartBossFight() {
        OnBossFightToggled?.Invoke(true);
        OnBossHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;

        OnBossHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        OnBossFightToggled?.Invoke(false);
        Destroy(gameObject);
    }
}