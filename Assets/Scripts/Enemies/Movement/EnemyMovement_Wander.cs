using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement_Wander : MonoBehaviour
{
    [Header("Wander Settings")]
    public float minChangeTime = 1f;
    public float maxChangeTime = 3f;

    private EnemyController brain;
    private Vector2 moveDirection;
    private float changeDirTimer;

    private void Awake() => brain = GetComponent<EnemyController>();

    private void Start()
    {
        PickNewDirection();
    }

    private void FixedUpdate()
    {
        if (brain.IsAttacking || brain.details.phases.Length == 0)
        {
            brain.Rb.linearVelocity = Vector2.MoveTowards(brain.Rb.linearVelocity, Vector2.zero, 15f * Time.deltaTime);
            return;
        }

        changeDirTimer -= Time.deltaTime;
        if (changeDirTimer <= 0f)
        {
            PickNewDirection();
        }

        float speed = brain.details.phases[0].movementSpeed;
        brain.Rb.linearVelocity = moveDirection * speed;
    }

    private void PickNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;

        changeDirTimer = Random.Range(minChangeTime, maxChangeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            Vector2 normal = collision.contacts[0].normal;
            moveDirection = Vector2.Reflect(moveDirection, normal).normalized;
        }
    }
}