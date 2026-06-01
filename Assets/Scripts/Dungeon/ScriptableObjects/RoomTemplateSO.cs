using UnityEngine;

[CreateAssetMenu(fileName = "RoomTemplate_", menuName = "Scriptable Objects/Room Template")]
public class RoomTemplateSO : ScriptableObject
{
    [Header("Room Setup")]
    [SerializeField] public RoomType roomType;

    [Header("Room Variants")]
    [Tooltip("Add all the different layout variations for this room type here!")]
    [SerializeField] public GameObject[] roomPrefabVariants;
}