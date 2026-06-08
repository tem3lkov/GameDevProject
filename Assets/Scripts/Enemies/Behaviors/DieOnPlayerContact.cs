using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class DieOnPlayerContact : MonoBehaviour
{
    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyController.Die();
        }
    }
}