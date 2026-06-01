using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EdgeDirection
{
    Up,
    Down,
    Left,
    Right
}

public class Room : MonoBehaviour
{
    [Header("Player Spawn Points")]
    [SerializeField] private Transform spawnUp;
    [SerializeField] private Transform spawnDown;
    [SerializeField] private Transform spawnLeft;
    [SerializeField] private Transform spawnRight;

    private int roomIndex;
    private RoomType roomType;
    public event Action OnPlayerEnteredRoom;
    private bool hasBeenEntered = false;
    private bool inCombat = false;

    public static event Action<Room> OnRoomEnteredGlobal;

    public Transform GetSpawnPoint(EdgeDirection arrivingAtDoor)
    {
        switch (arrivingAtDoor)
        {
            case EdgeDirection.Up: return spawnUp;
            case EdgeDirection.Down: return spawnDown;
            case EdgeDirection.Left: return spawnLeft;
            case EdgeDirection.Right: return spawnRight;
            default: return null;
        }
    }

    public int GetRoomIndex()
    {
        return roomIndex;
    }

    public RoomType GetRoomType()
    {
        return roomType;
    }

    public void SetupRoom(Cell currentCell, RoomTemplateSO template, int[] floorplan, List<Cell> cellList)
    {
        roomIndex = currentCell.Index;
        roomType = template.roomType;

        SetupOneByOne(currentCell, floorplan, cellList);
    }

    public void SetupOneByOne(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var currentCellIndex = cell.cellList[0];

        TryPlaceDoor(currentCellIndex, new Vector2(0, 3.6f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(0, -3.6f), EdgeDirection.Down, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(-6f, 0), EdgeDirection.Left, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(6f, 0), EdgeDirection.Right, floorplan, cellList, cell);

    }

    private RoomType GetDoorPriority(RoomType current, RoomType neighbor)
    {
        if (current == RoomType.Secret || neighbor == RoomType.Secret)
            return RoomType.Secret;

        if (current == RoomType.Boss || neighbor == RoomType.Boss)
            return RoomType.Boss;

        if (current == RoomType.Item || neighbor == RoomType.Item)
            return RoomType.Item;

        if (current == RoomType.Shop || neighbor == RoomType.Shop)
            return RoomType.Shop;

        return RoomType.Normal;
    }

    private void TryPlaceDoor(int fromIndex, Vector2 positionOffset, EdgeDirection direction, int[] floorplan, List<Cell> cellList, Cell currentCell)
    {
        int neighbourIndex = fromIndex + GetOffset(direction);

        if (neighbourIndex < 0 || neighbourIndex >= floorplan.Length) return;

        if (floorplan[neighbourIndex] != 1) return;

        var foundCell = cellList.FirstOrDefault(x => x.cellList.Contains(neighbourIndex));

        if (foundCell == null) return; // Extra safety check

        Door door;
        switch (GetDoorPriority(currentCell.roomType, foundCell.roomType))
        {
            case RoomType.Secret:
                door = Instantiate(RoomManager.Instance.secretDoorPrefab, transform);
                break;
            case RoomType.Item:
                door = Instantiate(RoomManager.Instance.lockedDoorPrefab, transform);
                break;
            default:
                door = Instantiate(RoomManager.Instance.normalDoorPrefab, transform);
                break;
        }
        door.transform.position = (Vector2)transform.position + positionOffset;

        RoomType correctDoorStyle = GetDoorPriority(currentCell.roomType, foundCell.roomType);

        SetupDoor(door, direction, correctDoorStyle);
    }

    private void SetupDoor(Door door, EdgeDirection direction, RoomType roomType)
    {
        var doorScript = GetDoorOptions(roomType);
        door.SetupDoor(roomIndex, direction, doorScript);
    }

    private DoorScript GetDoorOptions(RoomType roomType)
    {
        return RoomManager.Instance.doors.FirstOrDefault(x => x.roomType == roomType);
    }

    private int GetOffset(EdgeDirection direction)
    {
        switch (direction)
        {
            case EdgeDirection.Up: return 10;
            case EdgeDirection.Down: return -10;
            case EdgeDirection.Right: return 1;
            case EdgeDirection.Left: return -1;
        }
        return 0;
    }

    public void PlayerEntered()
    {
        OnRoomEnteredGlobal?.Invoke(this);

        if (hasBeenEntered) return;

        hasBeenEntered = true;

        OnPlayerEnteredRoom?.Invoke();
    }

    public void EnterCombat()
    {
        inCombat = true;
    }
    public void ExitCombat()
    {
        inCombat = false;
    }
    public bool IsInCombat()
    {
        return inCombat;
    }
}