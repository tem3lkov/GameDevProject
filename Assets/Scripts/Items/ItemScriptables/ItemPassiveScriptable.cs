using UnityEngine;
using System;

public enum StatType { Health, BlueHealth, MaxHealth, Speed, Damage, FireRate, ProjectileLifetime }

public class PassiveStats : EventArgs
{
    public int health = 0;
    public int blueHealth = 0;
    public int maxHealth = 0;
    public float speed = 0f;
    public float damage = 0f;
    public float tearsROF = 0f;
    public float tearsLifetime = 0f;
}
[CreateAssetMenu(fileName = "New Passive Item", menuName = "Scriptable Objects/Item/Passive Item")]
public class ItemPassiveScriptable : ItemScriptable
{
    public StatType statToModify;
    public float amount;
    [Tooltip("Saves and loads in the run data.")]
    public bool persistent;
    public static event Action<PassiveStats> OnStatsChanged;

    public override bool OnPickup(GameObject player)
    {
        if (statToModify == StatType.Health && !PlayerHealth.Instance.IsHealable((int)amount))
        {
            return false;
        }
        if (persistent)
        {
            PlayerInventory.Instance.GetPassiveItemNames().Add(itemName);
        }
        Activate(player);
        return true;
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
                stats.health = (int)amount;
                break;
            case StatType.BlueHealth:
                stats.blueHealth = (int)amount;
                break;
            case StatType.Damage:
                stats.damage = amount;
                break;
            case StatType.FireRate:
                stats.tearsROF = amount;
                break;
            case StatType.ProjectileLifetime:
                stats.tearsLifetime = amount;
                break;
            case StatType.MaxHealth:
                stats.maxHealth = (int)amount;
                break;
        }
        OnStatsChanged?.Invoke(stats);
    }
}
