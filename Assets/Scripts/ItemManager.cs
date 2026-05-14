using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private Item itemPrefab;
    [SerializeField] private ItemScriptable[] passiveItems;
    [SerializeField] private ItemScriptable[] activeItems;
    [SerializeField] private ItemScriptable[] coins = new ItemScriptable[3];
    [SerializeField] private ItemScriptable[] resources;
    public static ItemManager instance;

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

    public Item SpawnRandomItem(Vector2 position)
    {
        int randomIndex = Random.Range(0, passiveItems.Length + activeItems.Length + resources.Length);

        if (randomIndex < passiveItems.Length)
        {
            return SpawnRandomPassiveItem(position);
        }
        else if (randomIndex < passiveItems.Length + activeItems.Length)
        {
            return SpawnRandomActiveItem(position);
        }
        else
        {
            return SpawnRandomResource(position);
        }
    }
    public Item SpawnRandomNonResourceItem(Vector2 position)
    {
        int randomIndex = Random.Range(0, passiveItems.Length + activeItems.Length);

        if (randomIndex < passiveItems.Length)
        {
            return SpawnRandomPassiveItem(position);
        }
        else
        {
            return SpawnRandomActiveItem(position);
        }
    }
    public Item SpawnRandomPassiveItem(Vector2 position)
    {
        int randomIndex = Random.Range(0, passiveItems.Length);
        ItemScriptable randomItemScriptable = passiveItems[randomIndex];

        Item newItem = Instantiate(itemPrefab);
        newItem.Initialize(randomItemScriptable);
        newItem.transform.position = position;

        return newItem;
    }
    public Item SpawnRandomActiveItem(Vector2 position)
    {
        int randomIndex = Random.Range(0, activeItems.Length);
        ItemScriptable randomItemScriptable = activeItems[randomIndex];

        Item newItem = Instantiate(itemPrefab);
        newItem.Initialize(randomItemScriptable);
        newItem.transform.position = position;

        return newItem;
    }
    public Item SpawnRandomResource(Vector2 position)
    {
        int randomIndex = Random.Range(0, resources.Length);
        ItemScriptable randomItemScriptable = resources[randomIndex];

        Item newItem = Instantiate(itemPrefab);
        newItem.Initialize(randomItemScriptable);
        newItem.transform.position = position;

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
                coinIndex = 1; // Uncommon coin
                break;
            case 10:
                coinIndex = 2; // Rare coin
                break;
            default:
                coinIndex = 0; // Default to common coin
                break;
        }
        ItemScriptable coinToSpawn = coins[coinIndex];        
        
        var newCoin = Instantiate(itemPrefab, position, Quaternion.identity);
        newCoin.Initialize(coinToSpawn);

        return newCoin;
    }
    
}
