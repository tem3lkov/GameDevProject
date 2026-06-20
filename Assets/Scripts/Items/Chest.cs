using UnityEngine;

public enum ChestType
{
    Normal,
    Golden
}

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(Rigidbody2D))]
public class Chest : MonoBehaviour
{
    [Header("Chest Settings")]
    public ChestType chestType;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.linearDamping = 5f;
        rb.freezeRotation = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryOpenChest();
        }
    }

    private void TryOpenChest()
    {
        if (chestType == ChestType.Golden)
        {
            if (PlayerInventory.Instance.keys > 0)
            {
                PlayerInventory.Instance.AddResource(ResourceType.Key, -1);
                Debug.Log("Golden Chest opened! -1 Key.");
                OpenChest();
            } else
            {
                Debug.Log("You need a key! Pushing chest instead.");
            }
        } else
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        GetComponent<Collider2D>().enabled = false;

        SpawnLoot();

        Destroy(gameObject);
    }

    private void SpawnLoot()
    {
        Vector2 baseSpawnPos = transform.position;

        if (chestType == ChestType.Normal)
        {
            int resourceCount = Random.Range(1, 4);
            for (int i = 0; i < resourceCount; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * 1.6f;

                Item droppedItem = ItemManager.Instance.SpawnRandomResource(baseSpawnPos + randomOffset, false);

                if (droppedItem != null)
                {
                    droppedItem.SetPickupDelay(0.5f);
                }
            }
        } else if (chestType == ChestType.Golden)
        {
            Item droppedItem = ItemManager.Instance.SpawnRandomNonResourceItem(baseSpawnPos, false);

            if (droppedItem != null)
            {
                droppedItem.SetPickupDelay(0.5f);
            }
        }
    }
}