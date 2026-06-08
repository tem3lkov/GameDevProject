using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Resource")]
public class ResourceScriptable : ItemScriptable
{
    [SerializeField] public ResourceType resourceType;
    [SerializeField] public int amount;

    public override void OnPickup(GameObject player)
    {
        PlayerInventory.Instance.AddResource(resourceType, amount);
    }
    public override void Activate(GameObject player) {}
}
