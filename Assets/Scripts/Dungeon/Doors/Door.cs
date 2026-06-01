using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Door : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public DoorScript doorTypes;
    protected EdgeDirection direction;
    protected int currentRoomIndex;
    protected Collider2D col;

    private void Awake()
    {
        if (col == null)
            col = GetComponent<Collider2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        Room.OnRoomEnteredGlobal += ForceOpen;
    }
    private void OnDisable()
    {
        Room.OnRoomEnteredGlobal -= ForceOpen;
    }

    public void SetupDoor(int roomIndex, EdgeDirection dir, DoorScript doorData)
    {
        currentRoomIndex = roomIndex;
        direction = dir;
        doorTypes = doorData;
        SetDoorSprite();
    }
    public void SetDoorSprite()
    {
        DoorSpriteSet activeDoorSet = null;

        switch (direction)
        {
            case EdgeDirection.Up: activeDoorSet = doorTypes.upDoor; break;
            case EdgeDirection.Down: activeDoorSet = doorTypes.downDoor; break;
            case EdgeDirection.Left: activeDoorSet = doorTypes.leftDoor; break;
            case EdgeDirection.Right: activeDoorSet = doorTypes.rightDoor; break;
        }

        if (activeDoorSet != null)
        {
            spriteRenderer.sprite = isOpened() ? activeDoorSet.open : activeDoorSet.closed;
        }

        var colliderBounds = GetComponent<BoxCollider2D>();
        if (spriteRenderer.sprite != null)
        {
            Vector2 spriteSize = spriteRenderer.bounds.size;
            colliderBounds.size = new Vector2(spriteSize.x * 0.5f, spriteSize.y * 0.5f);
        }
    }

    public void EncounterLock()
    {
        col.enabled = false;
        CloseDoor();
    }
    public void EncounterUnlock()
    {
        col.enabled = true;
        if (CanAutoOpen() || isOpened())
        {
            TryOpen();
        }
    }
    public void CloseDoor()
    {
        SetDoorSprite();
    }
    public virtual bool CanAutoOpen()
    {
        return true;
    }
    public virtual bool TryOpen()
    {
        if (isOpened())
        {
            OpenDoor();
            return true;
        }
        return false;
    }
    public virtual bool isOpened()
    {
        return !RoomInCombat();
    }
    protected bool RoomInCombat()
    {
        return RoomManager.Instance.GetRoomAtCellIndex(currentRoomIndex) && RoomManager.Instance.GetRoomAtCellIndex(currentRoomIndex).IsInCombat()
        || false;
    }
    protected virtual void ForceOpen(Room room)
    {
        if (room.GetRoomIndex() != currentRoomIndex) return;
        OpenDoor();
    }
    protected void OpenDoor()
    {
        SetDoorSprite();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!TryOpen()) return;

        int targetRoomIndex;
        EdgeDirection destinationDoorDirection;

        switch (direction)
        {
            case EdgeDirection.Up:
                targetRoomIndex = currentRoomIndex + 10;
                destinationDoorDirection = EdgeDirection.Down;
                break;
            case EdgeDirection.Down:
                targetRoomIndex = currentRoomIndex - 10;
                destinationDoorDirection = EdgeDirection.Up;
                break;
            case EdgeDirection.Left:
                targetRoomIndex = currentRoomIndex - 1;
                destinationDoorDirection = EdgeDirection.Right;
                break;
            case EdgeDirection.Right:
                targetRoomIndex = currentRoomIndex + 1;
                destinationDoorDirection = EdgeDirection.Left;
                break;
            default: return;
        }

        Room targetRoom = RoomManager.Instance.GetRoomAtCellIndex(targetRoomIndex);
        if (targetRoom == null) return;

        Transform targetSpawn = targetRoom.GetSpawnPoint(destinationDoorDirection);

        if (targetSpawn != null)
        {
            other.transform.position = targetSpawn.position;
        }

        targetRoom.PlayerEntered();
    }
}