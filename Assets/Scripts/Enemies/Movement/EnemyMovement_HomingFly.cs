using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement_HomingFly : MonoBehaviour
{
    private EnemyController brain;

    private void Awake() => brain = GetComponent<EnemyController>();

    private void FixedUpdate()
    {
        if (brain.IsAttacking || brain.Target == null || brain.details.phases == null || brain.details.phases.Length == 0)
        {
            brain.Rb.linearVelocity = Vector2.MoveTowards(brain.Rb.linearVelocity, Vector2.zero, 15f * Time.deltaTime);
            return;
        }

        Vector2 direction = (brain.Target.position - transform.position).normalized;

        direction.x += Mathf.Sin(Time.time * 15f) * 0.2f;
        direction.y += Mathf.Cos(Time.time * 15f) * 0.2f;

        brain.Rb.linearVelocity = direction.normalized * brain.GetCurrentSpeed();
    }
}