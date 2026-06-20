using UnityEngine;
using System;
using System.Collections;

public class Item : MonoBehaviour
{
    protected ItemScriptable data;
    protected bool isForPurchase;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Collider2D col;
    protected bool recentlyDropped = false;

    public static event Action<ItemScriptable> OnItemPickedUp;

    private void Awake()
    {
        if (col == null)
            col = GetComponent<Collider2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (var hit in hits)
        {
            Room room = hit.GetComponentInParent<Room>();

            if (room != null)
            {
                transform.SetParent(room.transform);
                break;
            }
        }
    }

    public void SetPickupDelay(float delay = 2f)
    {
        StartCoroutine(PickupDelayCoroutine(delay));
    }

    protected IEnumerator PickupDelayCoroutine(float delay)
    {
        recentlyDropped = true;
        col.enabled = false;

        yield return new WaitForSeconds(delay);

        recentlyDropped = false;
        col.enabled = true;
    }

    
    public virtual void Initialize(ItemScriptable itemData, bool forPurchase)
    {
        data = itemData;
        SetItemSprite(data.itemSprite);

        isForPurchase = forPurchase;
    }
    protected virtual void SetItemSprite(Sprite item)
    {
        spriteRenderer.sprite = item;
        spriteRenderer.sortingOrder = 10;
        var colliderBounds = GetComponent<BoxCollider2D>();
        if (colliderBounds != null && colliderBounds.size == Vector2.zero)
        {
            colliderBounds.size = spriteRenderer.bounds.size;
        }
            
    }
    public ItemScriptable GetInventoryItemData() => data;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !collision.isTrigger || recentlyDropped) return;

        if (isForPurchase && PlayerInventory.Instance.coins < data.itemPrice)
        {
            Debug.Log("You only have " + PlayerInventory.Instance.coins + "c.");
            return;
        }

        data.OnPickup(collision.gameObject);

        if (isForPurchase) PlayerInventory.Instance.AddResource(ResourceType.Coin, -data.itemPrice);
        Debug.Log("Spent "+ (isForPurchase?data.itemPrice:0) + "c. Remaining " + 
                  PlayerInventory.Instance.coins + "c. Item collected: " + data.itemName);            
        OnItemPickedUp?.Invoke(data);

        // This destroys the item AND the child text!
        Destroy(gameObject);
    }
}