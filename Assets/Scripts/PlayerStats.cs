using UnityEngine;
using System;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private float health = 0;
    [SerializeField] private float damage = 0;
    [SerializeField] private float tearsROF = 0;
    public static event Action<float> OnSpeedChanged;
    public static event Action<float> OnHealthChanged;
    public static event Action<float> OnDamageChanged;
    public static event Action<float> OnTearsROFChanged;


    private void OnEnable()
    {
        ItemPassiveScriptable.OnStatsChanged += AddStats;
    }

    private void OnDisable()
    {
        ItemPassiveScriptable.OnStatsChanged -= AddStats;
    }
    void Start()
    {
        AddSpeed(200f);
        AddHealth(3f);
        AddDamage(2f);
        AddTearsROF(2f);
    }

    private void AddStats(PassiveStats boost)
    {
        AddSpeed(boost.speed);
        AddHealth(boost.health);
        AddDamage(boost.damage);
        AddTearsROF(boost.tearsROF);
    }
    private void AddSpeed(float newSpeed)
    {
        if (newSpeed == 0) return;
        speed += newSpeed;
        OnSpeedChanged?.Invoke(speed);
    }
    private void AddHealth(float newHealth)
    {
        if (newHealth == 0) return;
        health += newHealth;
        OnHealthChanged?.Invoke(health);
    }
    private void AddDamage(float newDamage)
    {
        if (newDamage == 0) return;
        damage += newDamage;
        OnDamageChanged?.Invoke(damage);
    }
    private void AddTearsROF(float newTears)
    {
        if (newTears == 0) return;
        tearsROF += newTears;
        OnTearsROFChanged?.Invoke(tearsROF);
    }


    public void ApplyFireRateBoost(float multiplier, float duration)
    {
        StartCoroutine(FireRateBoostCoroutine(multiplier, duration));
    }

    private IEnumerator FireRateBoostCoroutine(float multiplier, float duration)
    {
        tearsROF *= multiplier;
        OnTearsROFChanged?.Invoke(tearsROF);
        yield return new WaitForSeconds(duration);

        tearsROF /= multiplier;
        OnTearsROFChanged?.Invoke(tearsROF);
    }

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        StartCoroutine(DamageBoostCoroutine(multiplier, duration));
    }

    private IEnumerator DamageBoostCoroutine(float multiplier, float duration)
    {
        damage *= multiplier;
        OnDamageChanged?.Invoke(damage);
        yield return new WaitForSeconds(duration);

        damage /= multiplier;
        OnDamageChanged?.Invoke(damage);
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        speed *= multiplier;
        OnSpeedChanged?.Invoke(speed);
        yield return new WaitForSeconds(duration);

        speed /= multiplier;
        OnSpeedChanged?.Invoke(speed);
    }
}
