using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Scriptable Objects/Enemy Stats")]
public class EnemyStatsSO : ScriptableObject {
    public float maxHealth = 10f;
    public float movementSpeed = 3f;
    public float attackCooldown = 1f;
    public LayerMask obstacleMask;

    [Header("Damage Settings")]
    [Tooltip("1 = Half Heart, 2 = Full Heart")]
    public float damage = 1f;
}