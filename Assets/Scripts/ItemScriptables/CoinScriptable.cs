using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Resources/Coin")]
public class CoinScriptable : ItemScriptable
{
    public int amount;
    
    public override void OnPickup(GameObject player)
    {
        PlayerInventory.instance.AddResource(ResourceType.Coin, amount);
    }
    public override void Activate(GameObject player) {}
}
