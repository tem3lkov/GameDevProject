using UnityEngine;

[CreateAssetMenu(fileName = "Phase_", menuName = "Enemy Data/Boss Phase")]
public class BossPhaseSO : ScriptableObject
{
    public string phaseName = "Phase 1";

    [Range(0f, 1f)]
    [Tooltip("Phase triggers when health hits this %")]
    public float healthThreshold = 1f;

    public float movementSpeed = 3f;
    public float timeBetweenAttacks = 2f;

    public EnemyAttackSO[] allowedAttacks;
}