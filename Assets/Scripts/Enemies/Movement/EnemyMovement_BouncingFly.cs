using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement_BouncingFly : MonoBehaviour
{
    private EnemyController brain;
    private Vector2 currentDirection;
    private float speed;

    private void Awake() => brain = GetComponent<EnemyController>();

    private void Start()
    {
        if (brain.details.phases != null && brain.details.phases.Length > 0)
        {
            speed = brain.details.phases[0].movementSpeed;
        }
        else
        {
            speed = 3f; // Safe fallback
        }

        float randomX = Random.Range(0, 2) == 0 ? 1f : -1f;
        float randomY = Random.Range(0, 2) == 0 ? 1f : -1f;
        
        currentDirection = new Vector2(randomX, randomY).normalized;

        // 3. Kickstart the movement
        brain.Rb.linearVelocity = currentDirection * speed;
    }

    private void FixedUpdate()
    {
        if (brain.IsAttacking || brain.details.phases == null || brain.details.phases.Length == 0)
        {
            brain.Rb.linearVelocity = Vector2.MoveTowards(brain.Rb.linearVelocity, Vector2.zero, 15f * Time.deltaTime);
            return;
        }

        brain.Rb.linearVelocity = currentDirection * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount > 0)
        {
            Vector2 surfaceNormal = collision.GetContact(0).normal;
            
            currentDirection = Vector2.Reflect(currentDirection, surfaceNormal);

            brain.Rb.linearVelocity = currentDirection * speed;
        }
    }
}