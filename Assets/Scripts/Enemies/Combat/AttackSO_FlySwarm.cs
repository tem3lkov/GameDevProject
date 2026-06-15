using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FlySwarm", menuName = "Enemy Data/Attacks/Fly Swarm")]
public class AttackSO_FlySwarm : EnemyAttackSO
{
    [Header("Fly Movement")]
    [Tooltip("Колко бързо се приближава към играча")]
    public float forwardSpeed = 1.5f;
    [Tooltip("Скоростта на въртене в кръг")]
    public float circleSpeed = 5f;
    [Tooltip("Колко са широки кръговете")]
    public float circleSize = 2f;

    [Header("Layer Setup")]
    public string flyingLayer = "FlyingEnemy";

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        // Слагаме мухата на летящ слой, за да не се блъска в камъни
        int fLayerIndex = LayerMask.NameToLayer(flyingLayer);
        if (fLayerIndex != -1) enemy.gameObject.layer = fLayerIndex;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Move");

        float randomOffset = Random.Range(0f, 100f);

        while (true)
        {
            if (enemy.Target != null)
            {
                Vector2 dirToPlayer = (enemy.Target.position - enemy.transform.position).normalized;
                Vector2 forwardVelocity = dirToPlayer * forwardSpeed;

                float time = (Time.time + randomOffset) * circleSpeed;
                Vector2 circleVelocity = new Vector2(Mathf.Cos(time), Mathf.Sin(time)) * circleSize;

                enemy.Rb.linearVelocity = forwardVelocity + circleVelocity;
            } else
            {
                enemy.Rb.linearVelocity = Vector2.zero;
            }

            yield return null;
        }
    }
}