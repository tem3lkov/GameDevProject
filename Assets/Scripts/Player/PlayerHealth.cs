using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : SingletonMonoBehaviour<PlayerHealth>, IDamageable {
    [Header("Global Health Memory")]
    public static int globalMaxRedHalves = 6;
    public static int globalCurrentRedHalves = 6;
    public static int globalCurrentBlueHalves = 0;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    private float invincibilityTimer = 0f;
    private float invincibilityDuration = 2f;

    public static event Action<int, int, int> OnHealthChanged;

    private void OnEnable() {
        PlayerStats.OnHealthChanged += Heal;
    }
    private void OnDisable() {
        PlayerStats.OnHealthChanged -= Heal;
    }
    private void Start() {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateHealthUI();
    }

    private void Update() {
        if (invincibilityTimer > 0) invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(float amount) {
        if (invincibilityTimer > 0) return;

        int damageInHalves = Mathf.RoundToInt(amount);

        if (globalCurrentBlueHalves > 0) {
            int damageToBlue = Mathf.Min(damageInHalves, globalCurrentBlueHalves);
            globalCurrentBlueHalves -= damageToBlue;
            damageInHalves -= damageToBlue;
        }

        if (damageInHalves > 0) {
            globalCurrentRedHalves -= damageInHalves;
        }

        invincibilityTimer = invincibilityDuration;
        StartCoroutine(FlashRoutine());
        UpdateHealthUI();

        if (globalCurrentRedHalves <= 0) Die();
    }

    public void Heal(int red, int blue) {
        if (red > 0) HealRed(red);
        if (blue > 0) AddBlue(blue);    
    }

    public bool HealRed(int amountInHalves) {
        if (!IsHealable(amountInHalves)) return false; 

        globalCurrentRedHalves = Mathf.Min(globalCurrentRedHalves + amountInHalves, globalMaxRedHalves);
        UpdateHealthUI();
        return true;
    }

    public bool IsHealable(int amountInHalves) {
        return globalCurrentRedHalves + amountInHalves <= globalMaxRedHalves;
    }

    public void AddBlue(int amountInHalves) {
        globalCurrentBlueHalves += amountInHalves;
        UpdateHealthUI();
    }

    private void UpdateHealthUI() {
        OnHealthChanged?.Invoke(globalCurrentRedHalves, globalMaxRedHalves, globalCurrentBlueHalves);
    }

    private IEnumerator FlashRoutine() {
        while (invincibilityTimer > 0) {
            if (spriteRenderer != null) spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }

    private void Die() {
        Debug.Log("PLAYER DIED! Game Over.");

        globalMaxRedHalves = 6;
        globalCurrentRedHalves = 6;
        globalCurrentBlueHalves = 0;

        SceneManager.LoadScene("SampleScene");
    }
}