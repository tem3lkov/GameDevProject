using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Active Items/BoostTank")]
public class BoostTank : ItemActiveScriptable
{
    public float damageMultiplier = 3f;
    public float speedMultiplier = 0.5f;
    public float duration = 5f;

    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerCombat>().ApplyDamageBoost(damageMultiplier, duration);
        player.GetComponent<PlayerController>().ApplySpeedBoost(speedMultiplier, duration);
    }
}
