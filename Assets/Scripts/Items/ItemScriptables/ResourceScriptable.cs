using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Resource")]
public class ResourceScriptable : ItemScriptable
{
    [SerializeField] public ResourceType resourceType;
    [SerializeField] public int amount;

    public override bool OnPickup(GameObject player)
    {
        PlayerInventory.Instance.AddResource(resourceType, amount);
        return true;
    }
    public override void Activate(GameObject player) {}
}
