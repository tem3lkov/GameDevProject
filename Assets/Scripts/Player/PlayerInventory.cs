using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum ResourceType { Coin, Key, Bomb }
public class PlayerInventory : SingletonMonoBehaviour<PlayerInventory>
{
    [field: SerializeField] public int coins { get; private set; } = 0;
    [field: SerializeField] public int keys { get; private set; } = 1;
    [field: SerializeField] public int bombs { get; private set; } = 3;
    private int maxResources = 99;
    [SerializeField] private Explosion bombData;
    private ItemActiveScriptable currentItem;
    private List<string> passiveItems = new();
    private float cooldownTimer = 0f;

    public ItemActiveScriptable GetActiveItem()
    {
        return currentItem;
    }
    public List<string> GetPassiveItemNames()
    {
        return passiveItems;
    }

    public void ResetInventory()
    {
        coins = 0;
        keys = 1;
        bombs = 3;
        currentItem = null;
        passiveItems.Clear();
    }
    public void SetActiveItem(string itemName)
    {
        foreach (ItemScriptable item in ItemManager.Instance.GetActiveItems())
        {
            if (item.itemName == itemName)
            {
                if (!item.OnPickup(gameObject)) return;
                Debug.Log("Loaded active item "+itemName);
                break;
            }
        }
    }
    public void SetPassiveItems(List<string> itemNames)
    {
        ItemManager.Instance.GetPassiveItems();
        foreach (string name in itemNames)
        {
            foreach (ItemScriptable item in ItemManager.Instance.GetPassiveItems())
            {
                if (item.itemName == name)
                {
                    if (!item.OnPickup(gameObject)) return;
                    Debug.Log("Loaded passive item "+name);
                    break;
                }
            }
        }
    }

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

        Item dropped = Instantiate(ItemManager.Instance.GetItemPrefab(), transform.position, Quaternion.identity).GetComponent<Item>();

        bool forPurchase = false;
        dropped.SetPickupDelay(2f);
        dropped.Initialize(currentItem, forPurchase);
        currentItem = null;
    }

    public void SetBombs(int amount)
    {
        bombs = amount;
    }
    public void SetKeys(int amount)
    {
        keys = amount;
    }
    public void SetCoins(int amount)
    {
        coins = amount;
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

    private void TryExplodeBomb()
    {
        if (bombs <= 0)
            return;
            
        AddResource(ResourceType.Bomb, -1);


        Explosion bombInstance = Instantiate(bombData, transform.position, Quaternion.identity);
        bombInstance.TriggerExplode();
    }
}
