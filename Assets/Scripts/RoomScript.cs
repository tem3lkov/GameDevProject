using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Scriptable Objects/Room", order = 0)]
public class RoomScript : ScriptableObject
{
    [SerializeField] public RoomType roomType;
    [SerializeField] public Sprite roomSprite;
    
}
