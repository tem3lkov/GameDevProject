using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    private CinemachineConfiner2D confiner;
    private int currentRoomIndex;

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable() => RoomManager.OnMapGenerated += HandleNewMap;
    private void OnDisable() => RoomManager.OnMapGenerated -= HandleNewMap;

    private void HandleNewMap(Vector2 startPos)
    {
        currentRoomIndex = 45;
        MoveToRoom(currentRoomIndex);
    }

    public bool SetCurrentRoom(int index) {
        if (currentRoomIndex != index) {
            currentRoomIndex = index;
            MoveToRoom(currentRoomIndex);
            return true;
        }
        return false;
    }

    private void MoveToRoom(int index)
    {
        var room = RoomManager.instance.GetRoomAtCellIndex(index);

        if (room != null)
        {
            transform.position = new Vector3(room.transform.position.x, room.transform.position.y, transform.position.z);

            if (confiner == null) confiner = GetComponent<CinemachineConfiner2D>();

            var colliders = room.GetComponentsInChildren<Collider2D>();
            var bounds = colliders.FirstOrDefault(c => c.isTrigger);

            if (bounds != null)
            {
                confiner.BoundingShape2D = bounds;
                confiner.InvalidateBoundingShapeCache();
            }
        }
    }
}