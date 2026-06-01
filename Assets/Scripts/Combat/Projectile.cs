using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour {
    [Header("Settings")]
    [HideInInspector] public float damage;
    public float lifetime = 3f;

    [Header("Targeting")]
    public LayerMask targetLayers;
    public LayerMask whatDestroysTear;

    private void Start() {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if ((targetLayers.value & (1 << other.gameObject.layer)) > 0) {
            if (other.TryGetComponent<IDamageable>(out IDamageable hitTarget)) {
                hitTarget.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        if ((whatDestroysTear.value & (1 << other.gameObject.layer)) > 0) {
            Destroy(gameObject);
        }
    }
}