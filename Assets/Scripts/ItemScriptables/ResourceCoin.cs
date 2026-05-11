using UnityEngine;

public class ResourceCoin : Item
{
    CoinScriptable coinData;

    public void InitializeCoin(CoinScriptable coinData)
    {
        this.coinData = coinData;
        SetItemSprite(coinData.itemSprite);
    }

    public override void Initialize(ItemScriptable itemData)
    {
        if (itemData is CoinScriptable coinData)
        {
            InitializeCoin(coinData);
        }
        else
        {
            Debug.LogWarning("Invalid item data for ResourceCoin. Expected CoinScriptable.");
        }
    }
    protected override void SetItemSprite(Sprite item)
    {
        spriteRenderer.sprite = item;
        spriteRenderer.sortingOrder = 10;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBePickedUp) return;

        if (collision.CompareTag("Player"))
        {
            if (isCollected) return;
            isCollected = true;
            coinData.OnPickup(collision.gameObject);
            Debug.Log("Item collected: " + coinData.itemName);
            Destroy(gameObject);
        }
    }
}
