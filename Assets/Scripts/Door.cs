using UnityEngine;

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

    public void SetupDoor(int roomIndex, EdgeDirection dir, DoorScript doorData)
    {
        currentRoomIndex = roomIndex;
        direction = dir;
        doorTypes = doorData;
        SetDoorSprite();
    }
    public void SetDoorSprite()
    {
        switch (direction)
        {
            case EdgeDirection.Up:
                if (isOpened())
                    spriteRenderer.sprite = doorTypes.upDoor.open;
                else
                    spriteRenderer.sprite = doorTypes.upDoor.closed;
                break;
            case EdgeDirection.Down:
                if (isOpened())
                    spriteRenderer.sprite = doorTypes.downDoor.open;
                else
                    spriteRenderer.sprite = doorTypes.downDoor.closed;
                break;
            case EdgeDirection.Left:
                if (isOpened())
                    spriteRenderer.sprite = doorTypes.leftDoor.open;
                else
                    spriteRenderer.sprite = doorTypes.leftDoor.closed;
                break;
            case EdgeDirection.Right:
                if (isOpened())
                    spriteRenderer.sprite = doorTypes.rightDoor.open;
                else
                    spriteRenderer.sprite = doorTypes.rightDoor.closed;
                break;
        }
        var colliderBounds = GetComponent<BoxCollider2D>();
        if (spriteRenderer.sprite != null)
        {
            colliderBounds.size = spriteRenderer.bounds.size;
        }
    }
    
    public void EncounterLock() {
        col.enabled = false;
        CloseDoor();
    }
    public void EncounterUnlock() {
        col.enabled = true;
        if (CanAutoOpen() || isOpened())
        {
            TryOpen();
        }
    }
    public void CloseDoor() {
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
    public virtual bool isOpened() {
        return !RoomInCombat();
    }
    protected bool RoomInCombat() {
        return RoomManager.instance.GetRoomAtCellIndex(currentRoomIndex) && RoomManager.instance.GetRoomAtCellIndex(currentRoomIndex).IsInCombat()
        || false;
    }
    protected void OpenDoor() {
        SetDoorSprite();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!TryOpen()) return;
        if (CameraController.instance == null) return;

        int targetRoomIndex;
        switch (direction)
        {
            case EdgeDirection.Up:
                targetRoomIndex = currentRoomIndex+10;
                break;
            case EdgeDirection.Down:
                targetRoomIndex = currentRoomIndex-10;
                break;
            case EdgeDirection.Left:
                targetRoomIndex = currentRoomIndex-1;
                break;
            case EdgeDirection.Right:
                targetRoomIndex = currentRoomIndex+1;
                break;
            default:
                return;
        }

        bool changed = CameraController.instance.SetCurrentRoom(targetRoomIndex);
        if (!changed) return;

        PushPlayerIntoRoom(other.transform);

        RoomManager.instance.GetRoomAtCellIndex(targetRoomIndex).PlayerEntered();
    }
    private void PushPlayerIntoRoom(Transform player)
    {
        Vector2 pushDirection = direction switch
        {
            EdgeDirection.Up => Vector2.up,
            EdgeDirection.Down => Vector2.down,
            EdgeDirection.Left => Vector2.left,
            EdgeDirection.Right => Vector2.right,
            _ => Vector2.zero
        };

        float pushDistance = 5.5f;

        player.position += (Vector3)(pushDirection * pushDistance);
    }
}
