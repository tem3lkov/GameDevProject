using UnityEngine;
using System;

public enum StatType
{
    Speed,
    Health,
    Damage,
    TearsROF
}
public abstract class ItemPassive : Item
{
    // TODO merge all changes into a single event with class containing stat type and boost amount
    public static event Action<float> OnSpeedChanged;
    public static event Action<float> OnHealthChanged;
    public static event Action<float> OnDamageChanged;
    public static event Action<float> OnTearsROFChanged;

    protected void GiveHealthBoost(float boost)
    {
        OnHealthChanged?.Invoke(boost);
    }
    protected void GiveSpeedBoost(float boost)
    {
        OnSpeedChanged?.Invoke(boost);
    }
    protected void GiveDamageBoost(float boost)
    {
        OnDamageChanged?.Invoke(boost);
    }
    protected void GiveTearsBoost(float boost)
    {
        OnTearsROFChanged?.Invoke(boost);
    }
}
