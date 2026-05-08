using UnityEngine;

public class SecretDoor : Door, IDestructible
{
    private bool destroyed = false;

    public override void SetupDoor(int roomIndex, EdgeDirection dir, DoorScript doorData)
    {
        base.SetupDoor(roomIndex, dir, doorData);

        CloseDoor();
    }
    public override bool CanAutoOpen()
    {
        return true;
    }
    public override bool TryOpen()
    {
        if (!destroyed)
            return false;

        return base.TryOpen();
    }
    public override bool isOpened() {
        return destroyed;
    }

    public void UponDestruction()
    {
        destroyed = true;
        TryOpen();
    }
}