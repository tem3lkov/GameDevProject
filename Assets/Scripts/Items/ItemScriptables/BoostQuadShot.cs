using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item/Active Items/BoostQuadShot")]
public class BoostQuadShot : ItemActiveScriptable
{
    public ShootPattern quadShotPattern;
    public float duration;
    
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerCombat>().ApplyShootPattern(quadShotPattern, duration);
    }
}
