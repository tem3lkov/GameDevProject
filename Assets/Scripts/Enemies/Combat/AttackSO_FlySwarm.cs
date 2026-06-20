using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FlySwarm", menuName = "Enemy Data/Attacks/Fly Swarm")]
public class AttackSO_FlySwarm : EnemyAttackSO
{
    [Header("Fly Movement")]
    public float forwardSpeed = 1.5f;
    public float circleSpeed = 5f;
    public float circleSize = 2f;
    public string flyingLayer = "FlyingEnemy";

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        int fLayerIndex = LayerMask.NameToLayer(flyingLayer);
        if (fLayerIndex != -1) enemy.gameObject.layer = fLayerIndex;

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