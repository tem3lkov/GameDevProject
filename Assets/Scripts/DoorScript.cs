using UnityEngine;

[System.Serializable]
public class DoorSpriteSet
{
    public Sprite open;
    public Sprite closed;
}
[CreateAssetMenu(fileName = "Door", menuName = "Scriptable Objects/Door", order = 0)]
public class DoorScript : ScriptableObject
{
    [SerializeField] public RoomType roomType;
    [SerializeField] public DoorSpriteSet upDoor;
    [SerializeField] public DoorSpriteSet downDoor;
    [SerializeField] public DoorSpriteSet leftDoor;
    [SerializeField] public DoorSpriteSet rightDoor;

}