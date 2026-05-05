using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Scriptable Objects/Enemy Stats")]
public class EnemyStatsSO : ScriptableObject {
    public float maxHealth = 10f;
    public float movementSpeed = 3f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public LayerMask obstacleMask;
}