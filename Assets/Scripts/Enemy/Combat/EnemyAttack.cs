using System.Collections;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour {
    [Header("Range Settings")]
    public float minRange = 0f;
    public float maxRange = 5f;

    public virtual bool CanExecute(Enemy enemy) {
        if (enemy.Target == null) return false;

        float distance = Vector2.Distance(enemy.transform.position, enemy.Target.position);
        return distance >= minRange && distance <= maxRange;
    }

    public abstract IEnumerator Execute(Enemy user);
}