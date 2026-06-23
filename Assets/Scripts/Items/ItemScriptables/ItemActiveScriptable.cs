using UnityEngine;

public abstract class ItemActiveScriptable : ItemScriptable
{
    public float cooldownTime; // cooldownTime == 0 for single use

    public override bool OnPickup(GameObject player) 
    {
        PlayerInventory.Instance.PickupActiveItem(this);
        return true;
    }
    public virtual void OnDropDown(GameObject player) {}
}
