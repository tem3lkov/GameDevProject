using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CinemachineConfiner2D confiner;
    private int currentRoomIndex; 

    private void OnEnable() => RoomManager.OnMapGenerated += HandleNewMap;
    private void OnDisable() => RoomManager.OnMapGenerated -= HandleNewMap;

    private void HandleNewMap(Vector2 startPos)
    {
        currentRoomIndex = 45;
        MoveToRoom(currentRoomIndex);
    }

    private void Update()
    {
        if (Keyboard.current.shiftKey.IsPressed())
        {
            if (Keyboard.current.wKey.wasPressedThisFrame) TryMove(10);  // Up (Grid is 10 wide)
            if (Keyboard.current.sKey.wasPressedThisFrame) TryMove(-10); // Down
            if (Keyboard.current.aKey.wasPressedThisFrame) TryMove(-1);  // Left
            if (Keyboard.current.dKey.wasPressedThisFrame) TryMove(1); // Right
        }
    }

    private void TryMove(int offset)
    {
        int targetIndex = currentRoomIndex + offset;

        if (targetIndex >= 0 && targetIndex < 100 && MapGenerator.instance.getFloorCells[targetIndex] == 1)
        {
            currentRoomIndex = targetIndex;
            MoveToRoom(currentRoomIndex);
        }
    }

    private void MoveToRoom(int index)
    {
        var room = RoomManager.instance.GetRoomAtCellIndex(index);

        if (room != null)
        {
            transform.position = new Vector3(room.transform.position.x, room.transform.position.y, transform.position.z);

            if (confiner == null) confiner = GetComponent<CinemachineConfiner2D>();

            var bounds = room.GetComponentInChildren<Collider2D>();
            if (bounds != null)
            {
                confiner.BoundingShape2D = bounds;
                confiner.InvalidateBoundingShapeCache();
            }
        }
    }
}