using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class RoomManager : SingletonMonoBehaviour<RoomManager>
{
    private List<Room> createdRooms;

    [SerializeField] private float offsetX = 91;
    [SerializeField] private float offsetY = 60;

    public Door normalDoorPrefab;
    public DoorLocked lockedDoorPrefab;
    public SecretDoor secretDoorPrefab;
    [SerializeField] public DoorScript[] doors;

    [Header("Room Data")]
    [SerializeField] public RoomTemplateSO[] roomTemplates;

    public static event Action<Vector2> OnMapGenerated;

    protected override void Awake()
    {
        base.Awake();

        createdRooms = new List<Room>();
    }

    public Room GetRoomAtCellIndex(int index)
    {
        return createdRooms.Find(r => r.name == "Room_" + index);
    }

    public Vector2 GetStartRoomPosition()
    {
        if (createdRooms.Count > 0)
        {
            return createdRooms[0].transform.position;
        }
        return Vector2.zero;
    }

    public void SetupRooms(List<Cell> spawnedCells)
    {
        for (int i = createdRooms.Count - 1; i >= 0; i--)
        {
            Destroy(createdRooms[i].gameObject);
        }
        createdRooms.Clear();

        foreach (var currentCell in spawnedCells)
        {
            var template = roomTemplates.FirstOrDefault(r => r.roomType == currentCell.roomType);

            if (template == null || template.roomPrefabVariants == null || template.roomPrefabVariants.Length == 0)
            {
                Debug.LogWarning($"Missing template or prefabs for RoomType: {currentCell.roomType}");
                continue;
            }

            int randomVariantIndex = UnityEngine.Random.Range(0, template.roomPrefabVariants.Length);
            GameObject selectedPrefabVariant = template.roomPrefabVariants[randomVariantIndex];

            // Calculate position
            var currentPosition = currentCell.transform.position;
            var convertedPosition = new Vector2(currentPosition.x * offsetX, currentPosition.y * offsetY);

            GameObject spawnedRoomObj = Instantiate(selectedPrefabVariant, convertedPosition, Quaternion.identity);
            spawnedRoomObj.name = "Room_" + currentCell.Index;

            Room roomComponent = spawnedRoomObj.GetComponent<Room>();

            roomComponent.SetupRoom(currentCell, template, MapGenerator.Instance.getFloorCells, spawnedCells);

            createdRooms.Add(roomComponent);
        }

        OnMapGenerated?.Invoke(GetStartRoomPosition());

        Room startRoom = GetRoomAtCellIndex(45);
        if (startRoom != null)
        {
            startRoom.PlayerEntered();
        }

        GameManager.Instance.ChangeState(GameState.playingLevel);
    }
}