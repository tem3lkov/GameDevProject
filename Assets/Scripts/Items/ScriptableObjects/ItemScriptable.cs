using UnityEngine;

public abstract class ItemScriptable : ScriptableObject
{
    public string itemName;
    [TextArea(2, 4)] public string description;
    public int itemPrice;
    public Sprite itemSprite;
    public abstract void OnPickup(GameObject player);
    public abstract void Activate(GameObject player);
    public virtual bool PickUpable() { return true; }
}
