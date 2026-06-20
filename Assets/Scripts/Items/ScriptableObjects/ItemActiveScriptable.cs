using UnityEngine;

public abstract class ItemActiveScriptable : ItemScriptable
{
    public float cooldownTime; // cooldownTime == 0 for single use

    public override void OnPickup(GameObject player) 
    {
        PlayerInventory.Instance.PickupActiveItem(this);
    }
}
