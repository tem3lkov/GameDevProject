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
    [SerializeField] private SpriteRenderer spriteRenderer;
    private int roomIndex;
    private RoomType roomType;
    public event Action OnPlayerEnteredRoom;
    private bool hasBeenEntered = false;
    private bool inCombat = false;

    public static event Action<Room> OnRoomEnteredGlobal;
    public int GetRoomIndex()
    {
        return roomIndex;
    }

    public RoomType GetRoomType()
    {
        return roomType;
    }

    public void SetupRoom(Cell currentCell, RoomScript room)
    {
        roomIndex = currentCell.Index;
        spriteRenderer.sprite = room.roomSprite;

        roomType = room.roomType;

        var floorplan = MapGenerator.instance.getFloorCells;
        var cellList = MapGenerator.instance.getSpawnedCells;

        SetupOneByOne(currentCell, floorplan, cellList);
    }

    public void SetupOneByOne(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var currentCellIndex = cell.cellList[0];

        TryPlaceDoor(currentCellIndex, new Vector2(0, 3.7f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(0, -3.7f), EdgeDirection.Down, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(-6.2f, 0), EdgeDirection.Left, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(6.2f, 0), EdgeDirection.Right, floorplan, cellList, cell);

        if (roomType == RoomType.Item)
        {
            PlaceItem(currentCellIndex, new Vector2(0, 0));
        }
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

    private void PlaceItem(int fromIndex, Vector2 positionOffset)
    {
        var item = Instantiate(RoomManager.instance.itemPrefab, transform);
        item.transform.position = (Vector2)transform.position + positionOffset;

        SetItem(item);
    }

    private void SetItem(Item item)
    {
        int index = UnityEngine.Random.Range(0, RoomManager.instance.items.Length);
        var randomItem = RoomManager.instance.items[index];
        
        item.Initialize(randomItem);
    }
    

    private void TryPlaceDoor(int fromIndex, Vector2 positionOffset, EdgeDirection direction, int[] floorplan, List<Cell> cellList, Cell currentCell)
    {
        int neighbourIndex = fromIndex + GetOffset(direction);

        if (neighbourIndex < 0 || neighbourIndex >= floorplan.Length) return;

        if (floorplan[neighbourIndex] != 1) return;

        var foundCell = cellList.FirstOrDefault(x => x.cellList.Contains(neighbourIndex));

        Door door;
        switch (GetDoorPriority(currentCell.roomType, foundCell.roomType))
        {
            case RoomType.Secret:
                door = Instantiate(RoomManager.instance.secretDoorPrefab, transform);
                break;
            case RoomType.Item:
                door = Instantiate(RoomManager.instance.lockedDoorPrefab, transform);
                break;
            default:
                door = Instantiate(RoomManager.instance.normalDoorPrefab, transform);
                break;
        }
        door.transform.position = (Vector2)transform.position + positionOffset;

        RoomType correctDoorStyle = GetDoorPriority(currentCell.roomType, foundCell.roomType);

        SetupDoor(door, direction, correctDoorStyle);
    }

    private void SetupDoor(Door door, EdgeDirection direction, RoomType roomType)
    {
        var doorScript = GetDoorOptions(roomType);

        switch (direction)
        {
            case EdgeDirection.Up:
                if (door.isOpened())
                    door.SetupDoor(roomIndex, EdgeDirection.Up, doorScript);
                else
                    door.SetupDoor(roomIndex, EdgeDirection.Up, doorScript);
                break;

            case EdgeDirection.Down:
                if (door.isOpened())
                    door.SetupDoor(roomIndex, EdgeDirection.Down, doorScript);
                else
                    door.SetupDoor(roomIndex, EdgeDirection.Down, doorScript);
                break;

            case EdgeDirection.Left:
                if (door.isOpened())
                    door.SetupDoor(roomIndex, EdgeDirection.Left, doorScript);
                else
                    door.SetupDoor(roomIndex, EdgeDirection.Left, doorScript);
                break;

            case EdgeDirection.Right:
                if (door.isOpened())
                    door.SetupDoor(roomIndex, EdgeDirection.Right, doorScript);
                else
                    door.SetupDoor(roomIndex, EdgeDirection.Right, doorScript);
                break;
        }
    }

    private DoorScript GetDoorOptions(RoomType roomType)
    {
        return RoomManager.instance.doors.FirstOrDefault(x => x.roomType == roomType);
    }

    private int GetOffset(EdgeDirection direction)
    {
        switch (direction)
        {
            case EdgeDirection.Up:
                return 10;

            case EdgeDirection.Down:
                return -10;

            case EdgeDirection.Right:
                return 1;

            case EdgeDirection.Left:
                return -1;
        }

        return 0;
    }

    public void PlayerEntered() {
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