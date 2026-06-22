using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Active Items/BoostInvuln")]
public class BoostInvuln : ItemActiveScriptable
{
    public float invulnDuration = 2f;
    public float speedMultiplier = 2f;
    public float speedDuration = 2f;
    
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerHealth>().IncreaseInvincibility(invulnDuration);
        player.GetComponent<PlayerController>().ApplySpeedBoost(speedMultiplier, speedDuration);
    }
}
