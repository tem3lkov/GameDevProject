using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalJump", menuName = "Enemy Data/Attacks/Normal Jump")]
public class AttackSO_Jump : EnemyAttackSO
{
    [Header("Jump Physics")]
    public float hopForce = 6f;
    public float airTime = 0.5f;

    [Header("Landing Impact")]
    public float landingDamageRadius = 1.5f;
    public float landingDamage = 1f;

    [Header("Visual Colors (No Animation Fallback)")]
    public Color prepColor = Color.red;
    public Color airColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
    public override IEnumerator ExecuteAttack(EnemyController enemy)
    {
        enemy.Rb.linearVelocity = Vector2.zero;
        Collider2D myCollider = enemy.GetComponent<Collider2D>();

        Vector3 startPos = enemy.transform.position;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Prep");
        if (enemy.SpriteRend != null) enemy.SpriteRend.color = prepColor; 

        yield return new WaitForSeconds(0.2f);

        if (enemy.Target != null)
        {
            if (enemy.Anim != null) enemy.Anim.PlayAnimation("Jump");
            if (enemy.SpriteRend != null) enemy.SpriteRend.color = airColor;

            float originalDamping = enemy.Rb.linearDamping;
            enemy.Rb.linearDamping = 0f;

            if (myCollider != null) myCollider.enabled = false;

            Vector2 dir = (enemy.Target.position - enemy.transform.position).normalized;
            enemy.Rb.AddForce(dir * hopForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(airTime);

            enemy.Rb.linearDamping = originalDamping;
        }

        enemy.Rb.linearVelocity = Vector2.zero;

        Collider2D stuckInRock = Physics2D.OverlapCircle(enemy.transform.position, 0.2f, enemy.details.obstacleMask);
        if (stuckInRock != null)
        {
            RaycastHit2D hit = Physics2D.Linecast(startPos, enemy.transform.position, enemy.details.obstacleMask);

            if (hit.collider != null)
            {
                enemy.transform.position = hit.point + (hit.normal * 0.3f);
            }
        }

        if (myCollider != null) myCollider.enabled = true;
        if (enemy.SpriteRend != null) enemy.SpriteRend.color = Color.white;

        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Land");

        if (enemy.Target != null)
        {
            float distance = Vector2.Distance(enemy.transform.position, enemy.Target.position);
            if (distance <= landingDamageRadius)
            {
                if (enemy.Target.TryGetComponent(out IDamageable hit)) hit.TakeDamage(landingDamage);
            }
        }

        yield return new WaitForSeconds(0.3f);
        if (enemy.Anim != null) enemy.Anim.PlayAnimation("Idle");
    }
}