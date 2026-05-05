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
    public SpriteRenderer spriteRenderer;
    public int roomIndex;

    public RoomType roomType;

    [Header("Wall Door Blockers")]
    public GameObject blockerUp;
    public GameObject blockerDown;
    public GameObject blockerLeft;
    public GameObject blockerRight;

    [Header("Door Tracking")]
    public bool hasUpDoor;
    public bool hasDownDoor;
    public bool hasLeftDoor;
    public bool hasRightDoor;

    public event Action OnPlayerEnteredRoom;
    private bool hasBeenEntered = false;

    public void SetupRoom(Cell currentCell, RoomScript room)
    {
        roomIndex = currentCell.Index;
        spriteRenderer.sprite = room.roomSprite;

        //if (currentCell.roomType == RoomType.Secret) return;

        roomType = room.roomType;

        var floorplan = MapGenerator.instance.getFloorCells;
        var cellList = MapGenerator.instance.getSpawnedCells;

        SetupOneByOne(currentCell, floorplan, cellList);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && CameraController.instance != null) {
            if (CameraController.instance.SetCurrentRoom(roomIndex)) {
                Vector2 direction = (transform.position - other.transform.position).normalized;

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                    direction = new Vector2(Mathf.Sign(direction.x), 0);
                } else {
                    direction = new Vector2(0, Mathf.Sign(direction.y));
                }

                float pushDistance = 2.8f;

                other.transform.position += (Vector3)(direction * pushDistance);

                if (!hasBeenEntered) {
                    hasBeenEntered = true;
                    OnPlayerEnteredRoom?.Invoke(); 
                }
            }
        }
    }

    public void SetupOneByOne(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var currentCellIndex = cell.cellList[0];

        TryPlaceDoor(currentCellIndex, new Vector2(0, 3.61f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(currentCellIndex, new Vector2(0, -3.61f), EdgeDirection.Down, floorplan, cellList, cell);
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

        //if (foundCell.roomType == RoomType.Secret) return;

        var door = Instantiate(RoomManager.instance.doorPrefab, transform);
        door.transform.position = (Vector2)transform.position + positionOffset;

        RoomType correctDoorStyle = GetDoorPriority(currentCell.roomType, foundCell.roomType);

        SetupDoor(door, direction, correctDoorStyle);

        switch (direction) {
            case EdgeDirection.Up:
                if (blockerUp != null) blockerUp.SetActive(false);
                hasUpDoor = true; 
                break;

            case EdgeDirection.Down:
                if (blockerDown != null) blockerDown.SetActive(false);
                hasDownDoor = true;
                break;

            case EdgeDirection.Left:
                if (blockerLeft != null) blockerLeft.SetActive(false);
                hasLeftDoor = true;
                break;

            case EdgeDirection.Right:
                if (blockerRight != null) blockerRight.SetActive(false);
                hasRightDoor = true;
                break;
        }
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

    public void LockRoomDoors() {
        if (hasUpDoor && blockerUp != null) blockerUp.SetActive(true);
        if (hasDownDoor && blockerDown != null) blockerDown.SetActive(true);
        if (hasLeftDoor && blockerLeft != null) blockerLeft.SetActive(true);
        if (hasRightDoor && blockerRight != null) blockerRight.SetActive(true);
    }
    public void UnlockRoomDoors() {
        if (hasUpDoor && blockerUp != null) blockerUp.SetActive(false);
        if (hasDownDoor && blockerDown != null) blockerDown.SetActive(false);
        if (hasLeftDoor && blockerLeft != null) blockerLeft.SetActive(false);
        if (hasRightDoor && blockerRight != null) blockerRight.SetActive(false);
    }
}