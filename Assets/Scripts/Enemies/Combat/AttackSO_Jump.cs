using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalJump", menuName = "Enemy Data/Attacks/Normal Jump")]
public class AttackSO_Jump : EnemyAttackSO
{
    [Header("Jump Physics")]
    public float hopForce = 6f;
    public float airTime = 0.5f;
    public float landingClearance = 0.2f;

    [Header("Layer Setup")]
    public string defaultLayer = "GroundEnemy";
    public string flyingLayer = "FlyingEnemy";

    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);

        if (enemy.Target != null)
        {
            enemy.gameObject.layer = LayerMask.NameToLayer(flyingLayer);
            enemy.Rb.linearDamping = 0f;

            Vector2 dir = (enemy.Target.position - enemy.transform.position).normalized;
            AStarGrid grid = enemy.GetComponentInParent<RoomEncounter>()?.GetComponentInChildren<AStarGrid>();

            float finalForce = hopForce;
            float targetDist = hopForce * airTime;
            bool foundSpot = false;

            for (float d = targetDist; d > 0.5f; d -= 0.3f)
            {
                Vector2 testPos = (Vector2)enemy.transform.position + (dir * d);
                if (grid != null && Physics2D.OverlapCircle(testPos, landingClearance, enemy.details.obstacleMask) == null)
                {
                    finalForce = d / airTime;
                    foundSpot = true;
                    break;
                }
            }

            if (!foundSpot) finalForce = hopForce * 0.5f;

            enemy.Rb.AddForce(dir * finalForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(airTime);
        }

        enemy.Rb.linearVelocity = Vector2.zero;
        enemy.gameObject.layer = LayerMask.NameToLayer(defaultLayer);
        enemy.Rb.linearDamping = 1f;

        yield return new WaitForSeconds(0.3f);
    }
}