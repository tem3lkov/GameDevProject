using UnityEngine;
using UnityEngine.InputSystem;

public enum ResourceType { Coin, Key, Bomb }
public class PlayerInventory : SingletonMonoBehaviour<PlayerInventory>
{
    [SerializeField] private int coins = 0;
    [SerializeField] private int keys = 0;
    [SerializeField] private int bombs = 0;
    [SerializeField] private Explosion bombData;
    private int maxResources = 99;
    private ItemActiveScriptable currentItem;
    [SerializeField] private GameObject itemPrefab;
    private float cooldownTimer = 0f;
    
    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            UseItem();
        }
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryExplodeBomb();
        }
    }
    public void PickupActiveItem(ItemActiveScriptable newItem)
    {
        if (currentItem != null)
        {
            DropCurrentItem();
        }

        currentItem = newItem;
        cooldownTimer = 0f;
    }
    private void UseItem()
    {
        if (currentItem == null) return;
        if (cooldownTimer > 0f) return;

        currentItem.Activate(gameObject);

        cooldownTimer = currentItem.cooldownTime;

        if (currentItem.cooldownTime == 0f)
        {
            currentItem = null;
        }
    }

    private void DropCurrentItem()
    {
        if (currentItem == null) return;

        Item dropped = Instantiate(itemPrefab, transform.position, Quaternion.identity).GetComponent<Item>();

        bool forPurchase = false;
        dropped.SetPickupDelay(2f);
        dropped.Initialize(currentItem, forPurchase);
        currentItem = null;
    }

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Coin:
                coins = Mathf.Min(coins + amount, maxResources);
                break;
            case ResourceType.Key:
                keys = Mathf.Min(keys + amount, maxResources);
                break;
            case ResourceType.Bomb:
                bombs = Mathf.Min(bombs + amount, maxResources);
                break;
        }
    }

    public int GetResourceCount(ResourceType type)
    {
        return type switch
        {
            ResourceType.Coin => coins,
            ResourceType.Key => keys,
            ResourceType.Bomb => bombs,
            _ => 0
        };
    }
    private void TryExplodeBomb()
    {
        if (GetResourceCount(ResourceType.Bomb) <= 0)
            return;
            
        AddResource(ResourceType.Bomb, -1);


        Explosion bombInstance = Instantiate(bombData, transform.position, Quaternion.identity);
        bombInstance.TriggerExplode();
    }
}
