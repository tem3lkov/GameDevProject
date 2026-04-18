using UnityEngine;

public abstract class Item : MonoBehaviour
{
    //itemID?
    protected string itemName;
    protected string description;
    protected Sprite icon;//?
    protected GameObject itemPrefab;//?
    
    public abstract void Collect();
}
