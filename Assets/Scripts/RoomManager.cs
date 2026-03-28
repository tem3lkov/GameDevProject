using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    private List<Room> createdRooms;

    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;

    public Room roomPrefab;
    public Door doorPrefab;

    [SerializeField] public DoorScript[] doors;
    [SerializeField] public RoomScript[] rooms;

    public static RoomManager instance;

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

            spawnedRoom.SetupRoom(currentCell, foundRoom);
            
            createdRooms.Add(spawnedRoom);
        }
    }
}