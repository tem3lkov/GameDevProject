using UnityEngine;

public class SecretDoor : Door, IDestructible
{
    private bool destroyed = false;

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
    protected override void ForceOpen(Room room) {
        if (room.GetRoomType() != RoomType.Secret) return;
        destroyed = true;
        OpenDoor();
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