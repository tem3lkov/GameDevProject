using UnityEngine;

[CreateAssetMenu(fileName = "Bomb", menuName = "Scriptable Objects/Resources/Bomb")]
public class BombScriptable : ItemScriptable
{
    public float explosionRadius;
    public float explosionDamage;
    public float explosionDelay;
    public override void OnPickup(GameObject player)
    {
        int bombAmount = 1;
        player.GetComponent<PlayerInventory>().AddResource(ResourceType.Bomb, bombAmount);
    }

    public override void Activate(GameObject player)
    {
        Debug.Log("Bomb activated!");
        // TODO instantiate bomb, timer and boom
    }
}
