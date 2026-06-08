using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement_HoverNearSpawn : MonoBehaviour
{
    private EnemyController brain;

    [Tooltip("How far from its spawn point is it allowed to wander?")]
    public float wanderRadius = 1.5f;

    [Tooltip("How frequently does it change direction?")]
    public float changeDirectionInterval = 0.5f;

    private Vector2 spawnPosition;
    private Vector2 targetHoverPoint;
    private float timer;

    private void Awake() => brain = GetComponent<EnemyController>();

    private void Start()
    {
        spawnPosition = transform.position;
        PickNewHoverPoint();
    }

    private void FixedUpdate()
    {
        if (brain.IsAttacking || brain.details.phases.Length == 0)
        {
            brain.Rb.linearVelocity = Vector2.MoveTowards(brain.Rb.linearVelocity, Vector2.zero, 15f * Time.deltaTime);
            return;
        }

        timer -= Time.fixedDeltaTime;
        if (timer <= 0)
        {
            PickNewHoverPoint();
        }

        float speed = brain.details.phases[0].movementSpeed;
        Vector2 direction = (targetHoverPoint - (Vector2)transform.position).normalized;

        direction.x += Mathf.Sin(Time.time * 20f) * 0.1f;
        direction.y += Mathf.Cos(Time.time * 20f) * 0.1f;

        brain.Rb.linearVelocity = direction.normalized * speed;
    }

    private void PickNewHoverPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetHoverPoint = spawnPosition + randomOffset;
        timer = changeDirectionInterval;
    }
}