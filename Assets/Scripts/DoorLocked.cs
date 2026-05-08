using UnityEngine;

public class DoorLocked : Door
{
    private bool unlocked = false;

    public override void SetupDoor(int roomIndex, EdgeDirection dir, DoorScript doorData)
    {
        base.SetupDoor(roomIndex, dir, doorData);

        CloseDoor();
    }
    public override bool CanAutoOpen()
    {
        return false;
    }
    public override bool TryOpen()
    {
        TryUnlock();

        if (!unlocked)
            return false;

        return base.TryOpen();
    }
    public override bool isOpened() {
        return unlocked && base.isOpened();
    }

    public void TryUnlock()
    {
        if (PlayerInventory.instance.GetResourceCount(ResourceType.Key) <= 0) 
            return;

        PlayerInventory.instance.AddResource(ResourceType.Key, -1);
        unlocked = true;
    }
}