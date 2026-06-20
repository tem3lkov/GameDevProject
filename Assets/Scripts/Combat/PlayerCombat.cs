using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour {
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Combat Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float projectileLifetime = 1f;

    [Header("Isaac Mechanics")]
    public bool inheritPlayerMomentum = true;
    public float momentumMultiplier = 0.5f;

    [SerializeField] private float currentDamage = 2.5f;
    [SerializeField] private float currentTearsROF = 3f;

    private float nextFireTime;
    private Rigidbody2D rb;

    [SerializeField] private Transform firePoint;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        ItemPassiveScriptable.OnStatsChanged += UpdateTears;
    }
    private void OnDisable() {
        ItemPassiveScriptable.OnStatsChanged -= UpdateTears;
    }

    private void UpdateTears(PassiveStats newStats) {
        if (newStats.damage > 0) UpdateDamage(newStats.damage);
        if (newStats.tearsROF > 0) UpdateTearsROF(newStats.tearsROF);
        if (newStats.tearsLifetime > 0) UpdateProjectileLifetime(newStats.tearsLifetime);
    }

    private void UpdateDamage(float addDamage) {
        currentDamage += addDamage;
    }
    private void UpdateTearsROF(float addROF) {
        currentTearsROF = Mathf.Max(0.1f, currentTearsROF + addROF);
    }
    private void UpdateProjectileLifetime(float addLifetime) {
        projectileLifetime = Mathf.Max(0.1f, projectileLifetime + addLifetime); //??? behaviour when too low
    }
        
    public void ApplyFireRateBoost(float multiplier, float duration)
    {
        StartCoroutine(FireRateBoostCoroutine(multiplier, duration));
    }
    private IEnumerator FireRateBoostCoroutine(float multiplier, float duration)
    {
        currentTearsROF *= multiplier;
        yield return new WaitForSeconds(duration);

        currentTearsROF /= multiplier;
    }

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        StartCoroutine(DamageBoostCoroutine(multiplier, duration));
    }

    private IEnumerator DamageBoostCoroutine(float multiplier, float duration)
    {
        currentDamage *= multiplier;
        yield return new WaitForSeconds(duration);

        currentDamage /= multiplier;
    }

    private void Update() {
        HandleShooting();
    }

    private void HandleShooting() {
        if (Time.time < nextFireTime) return;

        Vector2 shootDirection = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed) shootDirection = Vector2.up;
        else if (Keyboard.current.downArrowKey.isPressed) shootDirection = Vector2.down;
        else if (Keyboard.current.leftArrowKey.isPressed) shootDirection = Vector2.left;
        else if (Keyboard.current.rightArrowKey.isPressed) shootDirection = Vector2.right;

        if (shootDirection != Vector2.zero) {
            Shoot(shootDirection);

            float cooldown = 1f / currentTearsROF;
            nextFireTime = Time.time + cooldown;
        }
    }

    private void Shoot(Vector2 direction) {
        if (projectilePrefab == null) return;

        if (audioSource != null && shootSound != null) {
            audioSource.PlayOneShot(shootSound);
        }

        GameObject tearObj = Instantiate(projectilePrefab, firePoint.position + new Vector3(direction.x, direction.y, 0).normalized * 0.25f, Quaternion.identity);

        if (tearObj.TryGetComponent<Projectile>(out Projectile proj)) {
            proj.damage = currentDamage;
            proj.lifetime = projectileLifetime;
        }

        if (tearObj.TryGetComponent<Rigidbody2D>(out Rigidbody2D projRb)) {
            Vector2 finalVelocity = direction * projectileSpeed;
            if (inheritPlayerMomentum && rb != null) {
                finalVelocity += rb.linearVelocity * momentumMultiplier;
            }
            projRb.linearVelocity = finalVelocity;
        }
    }
}