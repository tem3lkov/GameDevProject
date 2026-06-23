using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum ShootPattern { Standard, Triple, Plus }

public class PlayerCombat : MonoBehaviour {
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Combat Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float projectileLifetime = 1f;
    [SerializeField] private float projectileLifetimeMultiplier = 1f;
    [SerializeField] private ShootPattern projectilePattern = ShootPattern.Standard;

    [Header("Isaac Mechanics")]
    public bool inheritPlayerMomentum = true;
    public float momentumMultiplier = 0.5f;

    [SerializeField] private float currentDamage = 2.5f;
    [SerializeField] private float damageMultiplier = 1f;

    [SerializeField] private float currentTearsROF = 3f;
    [SerializeField] private float firerateMultiplier = 1f;

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
        projectileLifetime = Mathf.Max(0.1f, projectileLifetime + addLifetime);
    }
        
    public void ApplyFireRateBoost(float multiplier, float duration)
    {
        StartCoroutine(FireRateBoostCoroutine(multiplier, duration));
    }
    private IEnumerator FireRateBoostCoroutine(float multiplier, float duration)
    {
        firerateMultiplier *= multiplier;
        yield return new WaitForSeconds(duration);

        firerateMultiplier /= multiplier;
    }

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        StartCoroutine(DamageBoostCoroutine(multiplier, duration));
    }

    private IEnumerator DamageBoostCoroutine(float multiplier, float duration)
    {
        damageMultiplier *= multiplier;
        yield return new WaitForSeconds(duration);

        damageMultiplier /= multiplier;
    }
        
    public void SetProjectileLifetimeBoost(float multiplier)
    {
        projectileLifetimeMultiplier = multiplier;
    }

    public void ChangeShootPattern(ShootPattern pattern) {
        projectilePattern = pattern;
    }
    public void ApplyShootPattern(ShootPattern pattern, float duration)
    {
        StartCoroutine(ShootPatternCoroutine(pattern, duration));
    }
    private IEnumerator ShootPatternCoroutine(ShootPattern newPattern, float duration)
    {
        projectilePattern = newPattern;

        yield return new WaitForSeconds(duration);

        projectilePattern = ShootPattern.Standard;
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
            switch (projectilePattern) {
                case ShootPattern.Standard:
                    Shoot(shootDirection);
                    break;
                case ShootPattern.Triple:
                    Shoot(shootDirection);
                    if (shootDirection == Vector2.up || shootDirection == Vector2.down) {
                        Vector2 shootDirection2 = (3*shootDirection + Vector2.left).normalized;
                        Vector2 shootDirection3 = (3*shootDirection + Vector2.right).normalized;
                        Shoot(shootDirection2);
                        Shoot(shootDirection3);
                    }
                    else {
                        Vector2 shootDirection2 = (3*shootDirection + Vector2.up).normalized;
                        Vector2 shootDirection3 = (3*shootDirection + Vector2.down).normalized;
                        Shoot(shootDirection2);
                        Shoot(shootDirection3);
                    }
                    break;
                case ShootPattern.Plus:
                    Shoot(Vector2.up);
                    Shoot(Vector2.down);
                    Shoot(Vector2.left);
                    Shoot(Vector2.right);
                    break;
            }

            float cooldown = 1f / (currentTearsROF * firerateMultiplier);
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
            proj.damage = currentDamage * damageMultiplier;
            proj.lifetime = projectileLifetime * projectileLifetimeMultiplier;
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