using UnityEngine;

[CreateAssetMenu(fileName = "Door", menuName = "Scriptable Objects/Door", order = 0)]
public class DoorScript : ScriptableObject
{
    [SerializeField] public RoomType roomType;
    [SerializeField] public Sprite upDoor;
    [SerializeField] public Sprite downDoor;
    [SerializeField] public Sprite leftDoor;
    [SerializeField] public Sprite rightDoor;

}