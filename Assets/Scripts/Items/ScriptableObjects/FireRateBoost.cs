using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Active Items/FireRateBoost")]
public class FireRateBoostItem : ItemActiveScriptable
{
    public float multiplier = 2f;
    public float duration = 10f;

    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerStats>().ApplyFireRateBoost(multiplier, duration);
    }
}