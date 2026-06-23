using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

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

    public static event Action<int, int, int> OnResourcesUpdated;
    public static event Action<Sprite, bool> OnActiveItemChanged;
    public static event Action<float> OnCooldownUpdated;

    private void Start()
    {
        UpdateResourceUI();
        UpdateActiveItemUI();
        UpdateCooldownUI();
    }

    public void ResetInventory()
    {
        coins = 0;
        keys = 1;
        bombs = 3;
        currentItem = null;
        passiveItems.Clear();
    }
    public ItemActiveScriptable GetActiveItem() => currentItem;
    public List<string> GetPassiveItemNames() => passiveItems;

    public void SetActiveItem(string itemName)
    {
        foreach (ItemScriptable item in ItemManager.Instance.GetActiveItems())
        {
            if (item.itemName == itemName)
            {
                if (!item.OnPickup(gameObject)) return;
                Debug.Log("Loaded active item "+itemName);
                Debug.Log("Loaded active item " + itemName);
                UpdateActiveItemUI();
                UpdateCooldownUI();
                break;
            }
        }
    }

    public void SetPassiveItems(List<string> itemNames)
    {
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
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer < 0f) cooldownTimer = 0f;
            UpdateCooldownUI();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame) UseItem();
        if (Keyboard.current.eKey.wasPressedThisFrame) TryExplodeBomb();
    }

    public void PickupActiveItem(ItemActiveScriptable newItem)
    {
        if (currentItem != null) DropCurrentItem();

        currentItem = newItem;
        cooldownTimer = 0f;
        UpdateActiveItemUI();
        UpdateCooldownUI();
    }

    private void UseItem()
    {
        if (currentItem == null || cooldownTimer > 0f) return;

        currentItem.Activate(gameObject);
        cooldownTimer = currentItem.cooldownTime;

        UpdateCooldownUI();

        if (currentItem.cooldownTime == 0f)
        {
            currentItem = null;
            UpdateActiveItemUI();
        }
    }

    private void DropCurrentItem()
    {
        if (currentItem == null) return;

        Item dropped = Instantiate(ItemManager.Instance.GetItemPrefab(), transform.position, Quaternion.identity).GetComponent<Item>();
        dropped.SetPickupDelay(2f);
        dropped.Initialize(currentItem, false);
        ItemActiveScriptable activeItem = (ItemActiveScriptable)dropped.GetInventoryItemData();
        activeItem.OnDropDown(gameObject);

        currentItem = null;
        UpdateActiveItemUI();
        UpdateCooldownUI();
    }

    public void SetBombs(int amount) { bombs = amount; UpdateResourceUI(); }
    public void SetKeys(int amount) { keys = amount; UpdateResourceUI(); }
    public void SetCoins(int amount) { coins = amount; UpdateResourceUI(); }

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Coin: coins = Mathf.Min(coins + amount, maxResources); break;
            case ResourceType.Key: keys = Mathf.Min(keys + amount, maxResources); break;
            case ResourceType.Bomb: bombs = Mathf.Min(bombs + amount, maxResources); break;
        }
        UpdateResourceUI();
    }

    private void TryExplodeBomb()
    {
        if (bombs <= 0) return;

        AddResource(ResourceType.Bomb, -1);
        Explosion bombInstance = Instantiate(bombData, transform.position, Quaternion.identity);
        bombInstance.TriggerExplode();
    }

    private void UpdateResourceUI()
    {
        OnResourcesUpdated?.Invoke(coins, keys, bombs);
    }

    private void UpdateActiveItemUI()
    {
        Sprite itemIcon = currentItem != null ? currentItem.itemSprite : null;

        bool hasCooldown = currentItem != null && currentItem.cooldownTime > 0f;

        OnActiveItemChanged?.Invoke(itemIcon, hasCooldown);
    }

    private void UpdateCooldownUI()
    {
        if (currentItem == null || currentItem.cooldownTime == 0f)
        {
            OnCooldownUpdated?.Invoke(0f);
            return;
        }

        float fillPct = 1f - (cooldownTimer / currentItem.cooldownTime);
        OnCooldownUpdated?.Invoke(fillPct);
    }
}