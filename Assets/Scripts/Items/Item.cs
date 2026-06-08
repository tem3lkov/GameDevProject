using UnityEngine;
using System;
using System.Collections;

public enum StatType { Health, BlueHealth, Speed, Damage, FireRate }

public class Item : MonoBehaviour
{
    protected ItemScriptable data;
    protected bool isForPurchase;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Collider2D col;
    protected bool recentlyDropped = false;
    protected bool isCollected = false;

    private void Awake()
    {
        if (col == null)
            col = GetComponent<Collider2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (recentlyDropped || !data.PickUpable()) return;

        if (collision.CompareTag("Player"))
        {
            if (isCollected) return;

            // --- SHOP LOGIC CHECK ---
            if (isForPurchase)
            {
                // NOTE: Here is where you check the player's coin count against data.itemPrice
                // if (PlayerInventory.Coins < data.itemPrice) 
                // {
                //     Debug.Log("Not enough coins!");
                //     return; // Stops the player from picking it up
                // }
                // PlayerInventory.RemoveCoins(data.itemPrice);
            }

            isCollected = true;
            data.OnPickup(collision.gameObject);
            Debug.Log("Item collected: " + data.itemName);

            // This destroys the item AND the child text!
            Destroy(gameObject);
        }
    }
}