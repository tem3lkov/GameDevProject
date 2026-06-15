using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyDetails", menuName = "Enemy Data/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    public string enemyName = "Enemy";
    public bool isBoss = false;

    [Header("Core Stats")]
    public float maxHealth = 100f;
    public float damageToPlayer = 1f;
    public LayerMask obstacleMask;

    [Header("Combat Timings")]
    [Tooltip("How many seconds to wait before starting to attack after spawning")]
    public float initialAttackDelay = 0.3f;

    [Header("Phases")]
    [Tooltip("Normal enemies just need 1 phase. Bosses can have many!")]
    public BossPhaseSO[] phases;
}