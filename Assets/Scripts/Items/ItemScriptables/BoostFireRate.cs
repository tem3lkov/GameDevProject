using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Active Items/BoostFireRate")]
public class BoostFireRate : ItemActiveScriptable
{
    public float multiplier = 2f;
    public float duration = 10f;

    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerCombat>().ApplyFireRateBoost(multiplier, duration);
    }
}