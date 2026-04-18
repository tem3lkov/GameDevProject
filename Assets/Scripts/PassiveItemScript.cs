using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItem", menuName = "Scriptable Objects/PassiveItem", order = 0)]
public class PassiveItemScript : ScriptableObject
{
    [SerializeField] public StatType statType;
    [SerializeField] public float boostAmount;
    [SerializeField] public Sprite itemSprite;

}