using UnityEngine;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    [SerializeField] private Item itemPrefab;
    [SerializeField] private ItemScriptable[] passiveItems;
    [SerializeField] private ItemScriptable[] activeItems;
    [SerializeField] private ItemScriptable[] coins = new ItemScriptable[3];
    [SerializeField] private ItemScriptable[] resources;

    [Header("Spawn Settings")]
    [Tooltip("Walls and Obstacle Layer")]
    [SerializeField] private LayerMask obstacleMask;

    public Item SpawnRandomItem(Vector2 position, bool forPurchase)
    {
        int randomIndex = Random.Range(0, passiveItems.Length + activeItems.Length + resources.Length);

        if (randomIndex < passiveItems.Length) return SpawnRandomPassiveItem(position, forPurchase);
        else if (randomIndex < passiveItems.Length + activeItems.Length) return SpawnRandomActiveItem(position, forPurchase);
        else return SpawnRandomResource(position, forPurchase);
    }

    public Item SpawnRandomNonResourceItem(Vector2 position, bool forPurchase)
    {
        int randomIndex = Random.Range(0, passiveItems.Length + activeItems.Length);

        if (randomIndex < passiveItems.Length) return SpawnRandomPassiveItem(position, forPurchase);
        else return SpawnRandomActiveItem(position, forPurchase);
    }

    public Item SpawnRandomPassiveItem(Vector2 position, bool forPurchase)
    {
        int randomIndex = Random.Range(0, passiveItems.Length);
        ItemScriptable randomItemScriptable = passiveItems[randomIndex];

        Item newItem = Instantiate(itemPrefab);
        newItem.Initialize(randomItemScriptable, forPurchase);
        newItem.transform.position = FindSafeDropPosition(position);

        return newItem;
    }

    public Item SpawnRandomActiveItem(Vector2 position, bool forPurchase)
    {
        int randomIndex = Random.Range(0, activeItems.Length);
        ItemScriptable randomItemScriptable = activeItems[randomIndex];

        Item newItem = Instantiate(itemPrefab);
        newItem.Initialize(randomItemScriptable, forPurchase);
        newItem.transform.position = FindSafeDropPosition(position);

        return newItem;
    }

    public Item SpawnRandomResource(Vector2 position, bool forPurchase)
    {
        int randomIndex = Random.Range(0, resources.Length);
        ItemScriptable randomItemScriptable = resources[randomIndex];

        Item newItem = Instantiate(itemPrefab);
        newItem.Initialize(randomItemScriptable, forPurchase);

        newItem.transform.position = FindSafeDropPosition(position);

        return newItem;
    }

    public Item SpawnCoin(Vector2 position)
    {
        int chosenCoin = Random.Range(1, 10);
        int coinIndex;
        switch (chosenCoin)
        {
            case 8:
            case 9:
                coinIndex = 1;
                break;
            case 10:
                coinIndex = 2;
                break;
            default:
                coinIndex = 0;
                break;
        }
        ItemScriptable coinToSpawn = coins[coinIndex];

        Item newCoin = Instantiate(itemPrefab);
        newCoin.Initialize(coinToSpawn, false);
        newCoin.transform.position = FindSafeDropPosition(position);

        return newCoin;
    }

    private Vector2 FindSafeDropPosition(Vector2 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.3f, obstacleMask) == null)
            return targetPos;

        float searchRadius = 0.5f;
        for (int i = 0; i < 10; i++)
        {
            for (int angle = 0; angle < 360; angle += 45) 
            {
                float rad = angle * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * searchRadius;
                Vector2 potentialPos = targetPos + offset;

                if (Physics2D.OverlapCircle(potentialPos, 0.3f, obstacleMask) == null)
                {
                    return potentialPos;
                }
            }
            searchRadius += 0.5f;
        }

        return targetPos;
    }
}