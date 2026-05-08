using UnityEngine;
using UnityEngine.InputSystem;

public enum ResourceType { Coin, Key, Bomb }
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int coins = 0;
    [SerializeField] private CoinScriptable[] coinData = new CoinScriptable[3]; // more like the room should have the scriptable, or however they are spawned
    [SerializeField] private int keys = 0;
    [SerializeField] private int bombs = 0;
    [SerializeField] private BombScriptable bombData;
    private int maxResources = 99;
    private ItemActiveScriptable currentItem;
    [SerializeField] private GameObject itemPrefab;
    private float cooldownTimer = 0f;
    
    public static PlayerInventory instance;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
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

        Item dropped = Instantiate(itemPrefab, transform.position, Quaternion.identity).GetComponent<Item>();

        dropped.SetPickupDelay(2f);
        dropped.Initialize(currentItem);
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
        
        // TODO: Update your UI
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
        bombData.Activate(gameObject);
    }
}
