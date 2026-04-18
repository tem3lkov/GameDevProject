using UnityEngine;
using System;

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
        ItemPassive.OnSpeedChanged += AddSpeed;
        ItemPassive.OnHealthChanged += AddHealth;
        ItemPassive.OnDamageChanged += AddDamage;
        ItemPassive.OnTearsROFChanged += AddTearsROF;
    }

    private void OnDisable()
    {
        ItemPassive.OnSpeedChanged -= AddSpeed;
        ItemPassive.OnHealthChanged -= AddHealth;
        ItemPassive.OnDamageChanged -= AddDamage;
        ItemPassive.OnTearsROFChanged -= AddTearsROF;
    }
    void Start()
    {
        AddSpeed(200f);
        AddHealth(3f);
        AddDamage(5f);// adjust as needed
        AddTearsROF(1f);// adjust as needed
    }

    private void AddSpeed(float newSpeed)
    {
        speed += newSpeed;
        OnSpeedChanged?.Invoke(speed);
    }
    private void AddHealth(float newHealth)
    {
        health += newHealth;
        OnHealthChanged?.Invoke(health);
    }
    private void AddDamage(float newDamage)
    {
        damage += newDamage;
        OnDamageChanged?.Invoke(damage);
    }
    private void AddTearsROF(float newTears)
    {
        tearsROF += newTears;
        OnTearsROFChanged?.Invoke(tearsROF);
    }

}
