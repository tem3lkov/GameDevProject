using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement_Bounce : MonoBehaviour
{
    private EnemyController brain;
    private Vector2 currentDirection;

    private void Awake() => brain = GetComponent<EnemyController>();

    private void Start()
    {
        if (brain.Rb.linearVelocity.magnitude > 0.1f)
        {
            float signX = brain.Rb.linearVelocity.x >= 0 ? 1f : -1f;
            float signY = brain.Rb.linearVelocity.y >= 0 ? 1f : -1f;
            currentDirection = new Vector2(signX, signY).normalized;
        } else
        {
            float randomX = RunRNG.Range(0, 2) == 0 ? 1f : -1f;
            float randomY = RunRNG.Range(0, 2) == 0 ? 1f : -1f;
            currentDirection = new Vector2(randomX, randomY).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (brain.IsAttacking || brain.details.phases == null || brain.details.phases.Length == 0)
        {
            brain.Rb.linearVelocity = Vector2.MoveTowards(brain.Rb.linearVelocity, Vector2.zero, 15f * Time.deltaTime);
            return;
        }

        brain.Rb.linearVelocity = currentDirection * brain.GetCurrentSpeed();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((brain.details.obstacleMask.value & (1 << collision.gameObject.layer)) == 0) return;

        bool flippedX = false;
        bool flippedY = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;

            if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
            {
                if (!flippedX)
                {
                    currentDirection.x = Mathf.Sign(normal.x);
                    flippedX = true;
                }
            }
            else
            {
                if (!flippedY)
                {
                    currentDirection.y = Mathf.Sign(normal.y);
                    flippedY = true;
                }
            }
        }

        currentDirection.Normalize();

        brain.Rb.linearVelocity = currentDirection * brain.GetCurrentSpeed();
    }
}