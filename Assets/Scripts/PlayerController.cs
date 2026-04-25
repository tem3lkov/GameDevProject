using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerBody;
    private float speed;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        PlayerStats.OnSpeedChanged += UpdateSpeed;
        RoomManager.OnMapGenerated += SpawnPlayer;
    }

    private void OnDisable()
    {
        PlayerStats.OnSpeedChanged -= UpdateSpeed;
        RoomManager.OnMapGenerated -= SpawnPlayer;
    }
    private void UpdateSpeed(float newSpeed)
    {
        speed = newSpeed;
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
        playerBody.linearVelocity *= speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.TryGetComponent<Item>(out Item item))
        {
            item.Collect();
        }
    }

}
