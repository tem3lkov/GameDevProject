using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyMovement_AStar : MonoBehaviour
{
    private EnemyController brain;
    private List<Vector2> currentPath;
    private int currentWaypointIndex;

    [Tooltip("How often should the enemy recalculate the path? (Lower = faster reaction)")]
    public float pathUpdateInterval = 0.2f;

    [Tooltip("How close to the tile center before moving to the next? (Higher = smoother turns)")]
    public float nextWaypointDistance = 0.15f;

    private float nextPathUpdateTime;
    private AStarGrid myRoomGrid;

    private void Awake() => brain = GetComponent<EnemyController>();

    private void Start()
    {
        nextPathUpdateTime = Time.time + Random.Range(0f, pathUpdateInterval);
        myRoomGrid = GetComponentInParent<AStarGrid>();
    }

    private void FixedUpdate()
    {
        if (brain.IsAttacking || brain.Target == null || brain.details.phases.Length == 0)
        {
            brain.Rb.linearVelocity = Vector2.MoveTowards(brain.Rb.linearVelocity, Vector2.zero, 15f * Time.deltaTime);
            return;
        }

        if (Time.time >= nextPathUpdateTime)
        {
            CalculatePath();
            nextPathUpdateTime = Time.time + pathUpdateInterval;
        }

        FollowPath();
    }

    private void CalculatePath()
    {
        currentPath = AStarPathfinder.FindPath(myRoomGrid, transform.position, brain.Target.position);
        currentWaypointIndex = 0;
    }

    private void FollowPath()
    {
        if (currentPath == null || currentWaypointIndex >= currentPath.Count)
        {
            brain.Rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetWaypoint = currentPath[currentWaypointIndex];
        float currentSpeed = brain.details.phases[0].movementSpeed;
        Vector2 direction = (targetWaypoint - (Vector2)transform.position).normalized;
        brain.Rb.linearVelocity = direction * currentSpeed;

        if (Vector2.Distance(transform.position, targetWaypoint) < nextWaypointDistance)
        {
            currentWaypointIndex++;
        }
    }
}