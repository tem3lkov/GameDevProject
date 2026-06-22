using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Active Items/BoostDamage")]
public class BoostDamage : ItemActiveScriptable
{
    public float damageMultiplier = 2f;
    public float duration = 6f;
    
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerCombat>().ApplyDamageBoost(damageMultiplier, duration);
    }
}
