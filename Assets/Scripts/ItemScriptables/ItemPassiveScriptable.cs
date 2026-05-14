using UnityEngine;
using System;

public class PassiveStats : EventArgs
{
    public float speed = 0f;
    public float health = 0f;
    public float damage = 0f;
    public float tearsROF = 0f;
}
[CreateAssetMenu(fileName = "New Passive Item", menuName = "Scriptable Objects/Item/Passive Item")]
public class ItemPassiveScriptable : ItemScriptable
{
    public StatType statToModify;
    public float amount;
    public static event Action<PassiveStats> OnStatsChanged;

    public override void OnPickup(GameObject player)
    {
        Activate(player);
    }
    public override void Activate(GameObject player)
    {
        PassiveStats stats = new PassiveStats();
        switch (statToModify)
        {
            case StatType.Speed:
                stats.speed = amount;
                break;
            case StatType.Health:
                stats.health = amount;
                break;
            case StatType.Damage:
                stats.damage = amount;
                break;
            case StatType.FireRate:
                stats.tearsROF = amount;
                break;
        }
        OnStatsChanged?.Invoke(stats);
    }
}
