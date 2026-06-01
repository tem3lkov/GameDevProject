using System.Collections;
using UnityEngine;

public abstract class EnemyAttackSO : ScriptableObject
{
    public string attackName;
    public float minRange = 0f;
    public float maxRange = 10f;
    public float cooldownTime = 1f;

    public virtual bool CanExecute(EnemyController enemy)
    {
        if (enemy.Target == null) return false;
        float distance = Vector2.Distance(enemy.transform.position, enemy.Target.position);
        return distance >= minRange && distance <= maxRange;
    }

    public abstract IEnumerator ExecuteAttack(EnemyController enemy);
}