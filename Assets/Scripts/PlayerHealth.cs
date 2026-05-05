using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    
    private float invincibilityTimer = 0f;
    private float invincibilityDuration = 1f;

    private void Start()
    {
        currentHealth = 3f; 
    }

    private void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(float amount)
    {
        if (invincibilityTimer > 0) return;

        currentHealth -= amount;
        invincibilityTimer = invincibilityDuration;

        Debug.Log($"<color=orange>PLAYER TOOK DAMAGE!</color> Amount: {amount}. Remaining Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(0.5f); 
        }
    }

    private void Die()
    {
        SceneManager.LoadScene("SampleScene");
        Debug.Log("<color=red>PLAYER DIED!</color> Game Over.");
    }
}