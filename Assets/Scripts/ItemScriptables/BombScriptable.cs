using UnityEngine;

[CreateAssetMenu(fileName = "Bomb", menuName = "Scriptable Objects/Resources/Bomb")]
public class BombScriptable : ItemScriptable
{
    public float explosionRadius;
    public float explosionDamage;
    public float explosionDelay;
    [SerializeField] private GameObject bombPrefab;
    
    public override void OnPickup(GameObject player)
    {
        int bombAmount = 1;
        PlayerInventory.instance.AddResource(ResourceType.Bomb, bombAmount);
    }
    public override void Activate(GameObject player)
    {
        PlayerInventory inventory = PlayerInventory.instance;
        if (inventory.GetResourceCount(ResourceType.Bomb) <= 0)
            return;

        inventory.AddResource(ResourceType.Bomb, -1);

        GameObject bombObj = Object.Instantiate(bombPrefab, player.transform.position, Quaternion.identity);
        bombObj.GetComponent<ResourceBomb>().Initialize(this);
    }
}
