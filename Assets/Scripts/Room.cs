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
    public SpriteRenderer spriteRenderer;

    public void SetupRoom(Cell currentCell, RoomScript room)
    {
        spriteRenderer.sprite = room.roomSprite;

        //if (currentCell.roomType == RoomType.Secret) return;

        var floorplan = MapGenerator.instance.getFloorCells;
        var cellList = MapGenerator.instance.getSpawnedCells;

        SetupOneByOne(currentCell, floorplan, cellList);
    }

    public void SetupOneByOne(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var currentCellIndex = cell.cellList[0];

        TryPlaceDoor(currentCellIndex, new Vector2(0, 3.61f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(0, -3.61f), EdgeDirection.Down, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(-6f, 0), EdgeDirection.Left, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(6f, 0), EdgeDirection.Right, floorplan, cellList, cell);

        //PlaceItem(currentCellIndex, new Vector2(0, 0));
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
        //item prefab alongside item scriptable object are just about the passive items, should implement logic about active items too
        var item = Instantiate(RoomManager.instance.itemPrefab, transform);
        item.transform.position = (Vector2)transform.position + positionOffset;

        SetItem(item);
    }

    private void SetItem(Item item)
    {
        int index = Random.Range(0, RoomManager.instance.items.Length);
        var randomItem = RoomManager.instance.items[index];
        
        item.Initialize(randomItem);
    }
    

    private void TryPlaceDoor(int fromIndex, Vector2 positionOffset, EdgeDirection direction, int[] floorplan, List<Cell> cellList, Cell currentCell)
    {
        int neighbourIndex = fromIndex + GetOffset(direction);

        if (neighbourIndex < 0 || neighbourIndex >= floorplan.Length) return;

        if (floorplan[neighbourIndex] != 1) return;

        var foundCell = cellList.FirstOrDefault(x => x.cellList.Contains(neighbourIndex));

        if (foundCell.roomType == RoomType.Secret) return;

        var door = Instantiate(RoomManager.instance.doorPrefab, transform);
        door.transform.position = (Vector2)transform.position + positionOffset;

        RoomType correctDoorStyle = GetDoorPriority(currentCell.roomType, foundCell.roomType);

        SetupDoor(door, direction, correctDoorStyle);
    }

    private void SetupDoor(Door door, EdgeDirection direction, RoomType roomType)
    {
        var doorTypes = GetDoorOptions(roomType);

        switch (direction)
        {
            case EdgeDirection.Up:
                door.SetDoorSprite(doorTypes.upDoor);
                break;

            case EdgeDirection.Down:
                door.SetDoorSprite(doorTypes.downDoor);
                break;

            case EdgeDirection.Left:
                door.SetDoorSprite(doorTypes.leftDoor);
                break;

            case EdgeDirection.Right:
                door.SetDoorSprite(doorTypes.rightDoor);
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
}