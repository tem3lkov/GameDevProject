using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Active Items/BoostShotgun")]
public class BoostShotgun : ItemActiveScriptable
{
    public ShootPattern standardPattern;
    public ShootPattern shotgunPattern;
    public float projectileRangePenaltyMultiplier;
    private bool isShotgun = false;
    
    public override void Activate(GameObject player)
    {
        if (isShotgun) {
            player.GetComponent<PlayerCombat>().ChangeShootPattern(standardPattern);
            player.GetComponent<PlayerCombat>().SetProjectileLifetimeBoost(1);
            isShotgun = false;
        }
        else {
            player.GetComponent<PlayerCombat>().ChangeShootPattern(shotgunPattern);
            player.GetComponent<PlayerCombat>().SetProjectileLifetimeBoost(projectileRangePenaltyMultiplier);
            isShotgun = true;
        }
    }
    public override void OnDropDown(GameObject player) {
        if (isShotgun) {
            player.GetComponent<PlayerCombat>().ChangeShootPattern(standardPattern);
            player.GetComponent<PlayerCombat>().SetProjectileLifetimeBoost(1);
        }
    }
}
