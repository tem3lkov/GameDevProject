using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : SingletonMonoBehaviour<PlayerHealth>, IDamageable {
    [Header("Global Health Memory")]
    [field: SerializeField] public int globalMaxRedHalves { get; private set; } = 6;
    [field: SerializeField] public int globalCurrentRedHalves { get; private set; } = 6;
    [field: SerializeField] public int globalCurrentBlueHalves { get; private set; } = 0;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    private float invincibilityTimer = 0f;
    private float invincibilityDuration = 2f;

    public static event Action<int, int, int> OnHealthChanged;

    private void OnEnable() {
        ItemPassiveScriptable.OnStatsChanged += Heal;
    }
    private void OnDisable() {
        ItemPassiveScriptable.OnStatsChanged -= Heal;
    }
    private void Start() {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        UpdateHealthUI();
    }

    private void Update() {
        if (invincibilityTimer > 0) invincibilityTimer -= Time.deltaTime;
    }

    public void ResetHealth() {
        globalMaxRedHalves = 6;
        globalCurrentRedHalves = 6;
        globalCurrentBlueHalves = 0;
        UpdateHealthUI();
    }
    public void SetMaxHP(int amount)
    {
        globalMaxRedHalves = amount;
        UpdateHealthUI();
    }
    public void SetRedHP(int amount)
    {
        globalCurrentRedHalves = amount;
        UpdateHealthUI();
    }
    public void SetBlueHP(int amount)
    {
        globalCurrentBlueHalves = amount;
        UpdateHealthUI();
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
    public void IncreaseInvincibility(float duration) {
        invincibilityTimer += duration;
    }

    public void Heal(PassiveStats statChanges) {
        if (statChanges.health > 0) HealRed(statChanges.health);
        if (statChanges.blueHealth > 0) AddBlue(statChanges.blueHealth);    
        if (statChanges.maxHealth > 0) { AddMaxHealth(statChanges.maxHealth); HealRed(statChanges.maxHealth); }
    }

    public bool IsHealable(int amountInHalves) {
        return globalCurrentRedHalves + amountInHalves <= globalMaxRedHalves;
    }

    public bool HealRed(int amountInHalves) {
        if (!IsHealable(amountInHalves)) return false; 

        globalCurrentRedHalves = Mathf.Min(globalCurrentRedHalves + amountInHalves, globalMaxRedHalves);
        UpdateHealthUI();
        return true;
    }
    public void AddBlue(int amountInHalves) {
        globalCurrentBlueHalves += amountInHalves;
        UpdateHealthUI();
    }
    public void AddMaxHealth(int amountInHalves) {
        globalMaxRedHalves = Mathf.Max(1, globalMaxRedHalves + amountInHalves);
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

        GameManager.Instance.ChangeState(GameState.gameLost);
    }
}