using UnityEngine;

public class PassiveSpeed : ItemPassive
{
    [SerializeField] private float speedBoostAmount = 100f;

    private void Awake()
    {
        itemName = "Speed Boost";
        description = "Increases your speed by " + speedBoostAmount;
    }
    public override void Collect()
    {
        GiveSpeedBoost(speedBoostAmount);
        Destroy(gameObject);
    }
}
