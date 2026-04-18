using UnityEngine;

public abstract class ItemActive :  Item
{
    protected float cooldownTime;
    protected float currentCooldown;
    protected bool requiresCharges;

    protected abstract void Activate(PlayerController player);   
    
}
