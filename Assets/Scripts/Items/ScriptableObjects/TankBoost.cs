using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Active Items/TankBoost")]
public class TankBoost : ItemActiveScriptable
{
    public float damageMultiplier = 3f;
    public float speedMultiplier = 0.5f;
    public float duration = 5f;

    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerStats>().ApplyDamageBoost(damageMultiplier, duration);
        player.GetComponent<PlayerStats>().ApplySpeedBoost(speedMultiplier, duration);
    }
}
