using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class RoomManager : MonoBehaviour
{
    private List<Room> createdRooms;

    [SerializeField] private float offsetX = 91;
    [SerializeField] private float offsetY = 60;

    public Room roomPrefab;
    public Door normalDoorPrefab;
    public DoorLocked lockedDoorPrefab;
    public SecretDoor secretDoorPrefab;
    public Item itemPrefab;

    [SerializeField] public DoorScript[] doors;
    [SerializeField] public RoomScript[] rooms;
    [SerializeField] public ItemScriptable[] items;

    public static RoomManager instance;
    public static event Action<Vector2> OnMapGenerated;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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
        for(int i = createdRooms.Count - 1; i >= 0; i--)
        {
            Destroy(createdRooms[i].gameObject);
        }

        createdRooms.Clear();

        foreach(var currentCell in spawnedCells)
        {
            var foundRoom = rooms.FirstOrDefault(r => r.roomType == currentCell.roomType);
            
            var currentPosition = currentCell.transform.position;
            var convertedPosition = new Vector2(currentPosition.x * offsetX, currentPosition.y * offsetY);

            var spawnedRoom = Instantiate(roomPrefab, convertedPosition, Quaternion.identity);
            spawnedRoom.name = "Room_" + currentCell.Index;

            spawnedRoom.SetupRoom(currentCell, foundRoom);
            
            createdRooms.Add(spawnedRoom);
        }
        OnMapGenerated?.Invoke(GetStartRoomPosition());
    }
}