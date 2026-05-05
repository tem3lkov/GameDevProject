using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour {
    [Header("Combat Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;

    [Header("Isaac Mechanics")]
    public bool inheritPlayerMomentum = true;
    public float momentumMultiplier = 0.5f;

    private float currentDamage;
    private float currentTearsROF;

    private float nextFireTime;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        PlayerStats.OnDamageChanged += UpdateDamage;
        PlayerStats.OnTearsROFChanged += UpdateTearsROF;
    }

    private void OnDisable() {
        PlayerStats.OnDamageChanged -= UpdateDamage;
        PlayerStats.OnTearsROFChanged -= UpdateTearsROF;
    }

    private void UpdateDamage(float newDamage) {
        currentDamage = newDamage;
    }

    private void UpdateTearsROF(float newTears) {
        currentTearsROF = Mathf.Max(0.1f, newTears);
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

        GameObject tearObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        if (tearObj.TryGetComponent<Projectile>(out Projectile proj)) {
            proj.damage = currentDamage;
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