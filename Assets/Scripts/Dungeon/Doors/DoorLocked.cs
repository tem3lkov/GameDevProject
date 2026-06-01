using UnityEngine;

public class DoorLocked : Door
{
    private bool unlocked = false;

    public override bool CanAutoOpen()
    {
        return false;
    }
    public override bool TryOpen()
    {
        if (!unlocked && !TryUnlock())
            return false;

        return base.TryOpen();
    }
    protected override void ForceOpen(Room room) {
        if (room.GetRoomIndex() != currentRoomIndex) return;
        unlocked = true;
        OpenDoor();
    }
    public override bool isOpened() {
        return unlocked && base.isOpened();
    }

    public bool TryUnlock()
    {
        if (PlayerInventory.instance.GetResourceCount(ResourceType.Key) <= 0) 
            return false;

        PlayerInventory.instance.AddResource(ResourceType.Key, -1);
        unlocked = true;
        return true;
    }
}