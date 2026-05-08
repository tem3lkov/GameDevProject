using UnityEngine;
using System;
using System.Collections;

public enum StatType { Health, Speed, Damage, FireRate }

public class Item : MonoBehaviour
{
    private ItemScriptable data;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D col;
    private bool canBePickedUp = true;
    private bool isCollected = false;

    private void Awake()
    {
        if (col == null)
            col = GetComponent<Collider2D>();
    }

    public void SetPickupDelay(float delay = 2f)
    {
        StartCoroutine(PickupDelayCoroutine(delay));
    }

    private IEnumerator PickupDelayCoroutine(float delay)
    {
        canBePickedUp = false;
        col.enabled = false;

        yield return new WaitForSeconds(delay);

        canBePickedUp = true;
        col.enabled = true;
    }

    
    public void Initialize(ItemScriptable itemData)
    {
        data = itemData;
        SetItemSprite(data.itemSprite);
    }
    private void SetItemSprite(Sprite item)
    {
        spriteRenderer.sprite = item;
        spriteRenderer.sortingOrder = 10;
        var colliderBounds = GetComponent<BoxCollider2D>();
        colliderBounds.size = spriteRenderer.bounds.size;
    }
    public ItemScriptable GetInventoryItemData() => data;

    private void OnTriggerEnter2D(Collider2D collision)
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