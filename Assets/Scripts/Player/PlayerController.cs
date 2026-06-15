using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerBody;
    [SerializeField] private float speed = 200f;
    [SerializeField] private float speedMultiplier = 1f;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        ItemPassiveScriptable.OnStatsChanged += UpdateSpeed;
        RoomManager.OnMapGenerated += SpawnPlayer;
    }
    private void OnDisable()
    {
        ItemPassiveScriptable.OnStatsChanged -= UpdateSpeed;
        RoomManager.OnMapGenerated -= SpawnPlayer;
    }

    private void UpdateSpeed(PassiveStats statChanges)
    {
        if (statChanges.speed > 0) speed += statChanges.speed;
    }    
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        speedMultiplier *= multiplier;
        
        yield return new WaitForSeconds(duration);

        speedMultiplier /= multiplier;
    }

    private void SpawnPlayer(Vector2 startPos)
    {
        transform.position = startPos;
    }

    private void FixedUpdate() {
        Move();
    }
    private void Move()
    {
        playerBody.linearVelocity = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) 
        {
            playerBody.linearVelocity += Vector2.up;
        }
        if (Keyboard.current.sKey.isPressed) 
        {
            playerBody.linearVelocity += Vector2.down;
        }
        if (Keyboard.current.aKey.isPressed) 
        {
            playerBody.linearVelocity += Vector2.left;
        }
        if (Keyboard.current.dKey.isPressed) 
        {
            playerBody.linearVelocity += Vector2.right;
        }
        playerBody.linearVelocity = playerBody.linearVelocity.normalized;
        playerBody.linearVelocity *= speed * speedMultiplier * Time.fixedDeltaTime;
    }

}
