using UnityEngine;
using System;
using System.Collections;

public enum StatType { Health, Speed, Damage, FireRate }

public class Item : MonoBehaviour
{
    protected ItemScriptable data;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Collider2D col;
    protected bool canBePickedUp = true;
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
        canBePickedUp = false;
        col.enabled = false;

        yield return new WaitForSeconds(delay);

        canBePickedUp = true;
        col.enabled = true;
    }

    
    public virtual void Initialize(ItemScriptable itemData)
    {
        data = itemData;
        SetItemSprite(data.itemSprite);
    }
    protected void SetItemSprite(Sprite item)
    {
        spriteRenderer.sprite = item;
        spriteRenderer.sortingOrder = 10;
        var colliderBounds = GetComponent<BoxCollider2D>();
        colliderBounds.size = spriteRenderer.bounds.size;
    }
    public ItemScriptable GetInventoryItemData() => data;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBePickedUp) return;

        if (collision.CompareTag("Player"))
        {
            if (isCollected) return;
            isCollected = true;
            data.OnPickup(collision.gameObject);
            Debug.Log("Item collected: " + data.itemName);
            Destroy(gameObject);
        }
    }
}